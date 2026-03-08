import * as cdk from 'aws-cdk-lib';
import * as ec2 from 'aws-cdk-lib/aws-ec2';
import * as iam from 'aws-cdk-lib/aws-iam';
import * as ssm from 'aws-cdk-lib/aws-ssm';
import * as elasticloadbalancingv2 from 'aws-cdk-lib/aws-elasticloadbalancingv2';
import * as elasticloadbalancingv2targets from 'aws-cdk-lib/aws-elasticloadbalancingv2-targets';
import * as acm from 'aws-cdk-lib/aws-certificatemanager';
import { Construct } from 'constructs';

export interface LymmHolidayLetsStackProps extends cdk.StackProps {
  /** Logical environment name, e.g. "dev" | "staging" | "prod" */
  envName: string;
}

/**
 * LymmHolidayLetsStack
 *
 * Deploys the LymmHolidayLets .NET API on a single EC2 instance inside a VPC.
 *
 * Architecture overview:
 *  - VPC (2 AZs, public + private subnets)
 *  - Application Load Balancer (public, HTTPS on 443 + HTTP redirect on 80)
 *  - EC2 instance (Amazon Linux 2023) in a private subnet
 *  - Docker is installed via UserData; the API container is pulled from ECR
 *    or built from the local Dockerfile via `docker-compose up`
 *  - Secrets stored in SSM Parameter Store (no plain-text passwords in code)
 *  - IAM instance role with SSM Session Manager access (no SSH key required)
 *
 * Prerequisites before first deploy:
 *  1. Store secrets in SSM Parameter Store (see README.infra.md).
 *  2. Push your Docker image to ECR, or set USE_ECR=false to build on-box.
 *  3. Run `cdk bootstrap` once per account/region.
 *  4. (Optional) Create an ACM certificate and set ACM_CERTIFICATE_ARN env var.
 */
export class LymmHolidayLetsStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props: LymmHolidayLetsStackProps) {
    super(scope, id, props);

    const { envName } = props;

    // ── VPC ──────────────────────────────────────────────────────────────────
    const vpc = new ec2.Vpc(this, 'Vpc', {
      vpcName: `lymmholidaylets-${envName}-vpc`,
      maxAzs: 2,
      natGateways: 1,
      subnetConfiguration: [
        {
          cidrMask: 24,
          name: 'Public',
          subnetType: ec2.SubnetType.PUBLIC,
        },
        {
          cidrMask: 24,
          name: 'Private',
          subnetType: ec2.SubnetType.PRIVATE_WITH_EGRESS,
        },
      ],
    });

    // ── Security Groups ──────────────────────────────────────────────────────

    // ALB: accepts HTTP (redirect) and HTTPS from the internet.
    const albSg = new ec2.SecurityGroup(this, 'AlbSg', {
      vpc,
      securityGroupName: `lymmholidaylets-${envName}-alb-sg`,
      description: 'ALB inbound HTTP/HTTPS',
      allowAllOutbound: true,
    });
    albSg.addIngressRule(ec2.Peer.anyIpv4(), ec2.Port.tcp(80),  'HTTP from internet');
    albSg.addIngressRule(ec2.Peer.anyIpv4(), ec2.Port.tcp(443), 'HTTPS from internet');

    // EC2: only accepts traffic from the ALB on port 8080 (the API).
    const ec2Sg = new ec2.SecurityGroup(this, 'Ec2Sg', {
      vpc,
      securityGroupName: `lymmholidaylets-${envName}-ec2-sg`,
      description: 'EC2 – API traffic from ALB only',
      allowAllOutbound: true,
    });
    ec2Sg.addIngressRule(albSg, ec2.Port.tcp(8080), 'API from ALB');

    // ── IAM Role for EC2 ─────────────────────────────────────────────────────
    const instanceRole = new iam.Role(this, 'Ec2Role', {
      roleName: `lymmholidaylets-${envName}-ec2-role`,
      assumedBy: new iam.ServicePrincipal('ec2.amazonaws.com'),
      managedPolicies: [
        // Enables SSM Session Manager (no SSH key required).
        iam.ManagedPolicy.fromAwsManagedPolicyName('AmazonSSMManagedInstanceCore'),
        // Allows pulling from ECR.
        iam.ManagedPolicy.fromAwsManagedPolicyName('AmazonEC2ContainerRegistryReadOnly'),
        // Allows CloudWatch logs.
        iam.ManagedPolicy.fromAwsManagedPolicyName('CloudWatchAgentServerPolicy'),
      ],
    });

    // Allow reading SSM parameters under the app's namespace.
    instanceRole.addToPolicy(new iam.PolicyStatement({
      actions: ['ssm:GetParameter', 'ssm:GetParameters', 'ssm:GetParametersByPath'],
      resources: [
        `arn:aws:ssm:${this.region}:${this.account}:parameter/lymmholidaylets/${envName}/*`,
      ],
    }));

    // ── EC2 User Data ─────────────────────────────────────────────────────────
    // Runs on first boot to install Docker and start the API container.
    const userData = ec2.UserData.forLinux();
    userData.addCommands(
      '#!/bin/bash',
      'set -euxo pipefail',

      // System update & install Docker + jq
      'dnf update -y',
      'dnf install -y docker jq aws-cli',
      'systemctl enable --now docker',

      // Docker Compose v2 plugin
      'mkdir -p /usr/local/lib/docker/cli-plugins',
      'curl -SL "https://github.com/docker/compose/releases/latest/download/docker-compose-linux-x86_64" -o /usr/local/lib/docker/cli-plugins/docker-compose',
      'chmod +x /usr/local/lib/docker/cli-plugins/docker-compose',

      // Fetch secrets from SSM Parameter Store into environment variables.
      `export DB_PASSWORD=$(aws ssm get-parameter --name /lymmholidaylets/${envName}/db/password --with-decryption --query Parameter.Value --output text --region ${this.region})`,
      `export STRIPE_WEBHOOK_KEY=$(aws ssm get-parameter --name /lymmholidaylets/${envName}/stripe/webhookKey --with-decryption --query Parameter.Value --output text --region ${this.region})`,
      `export SENDGRID_API_KEY=$(aws ssm get-parameter --name /lymmholidaylets/${envName}/sendgrid/apiKey --with-decryption --query Parameter.Value --output text --region ${this.region})`,
      `export LOGZIO_TOKEN=$(aws ssm get-parameter --name /lymmholidaylets/${envName}/logzio/token --with-decryption --query Parameter.Value --output text --region ${this.region})`,

      // Write a .env file so docker-compose picks up secrets.
      'mkdir -p /opt/lymmholidaylets',
      'cat > /opt/lymmholidaylets/.env << EOF',
      'ASPNETCORE_ENVIRONMENT=Production',
      `ASPNETCORE_URLS=http://+:8080`,
      'DB_PASSWORD=${DB_PASSWORD}',
      'STRIPE_WEBHOOK_KEY=${STRIPE_WEBHOOK_KEY}',
      'SENDGRID_API_KEY=${SENDGRID_API_KEY}',
      'LOGZIO_TOKEN=${LOGZIO_TOKEN}',
      'EOF',

      // Write a minimal docker-compose.yml for the API only.
      // (The database runs on RDS or a separate host in production.)
      'cat > /opt/lymmholidaylets/docker-compose.yml << \'COMPOSE\'',
      'services:',
      '  lymmholidaylets-api:',
      '    image: ${ECR_IMAGE:-lymmholidaylets-api:latest}',
      '    restart: unless-stopped',
      '    ports:',
      '      - "8080:8080"',
      '    env_file:',
      '      - /opt/lymmholidaylets/.env',
      '    healthcheck:',
      '      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]',
      '      interval: 30s',
      '      timeout: 10s',
      '      retries: 5',
      '      start_period: 30s',
      'COMPOSE',

      // Authenticate to ECR and start the container.
      `aws ecr get-login-password --region ${this.region} | docker login --username AWS --password-stdin ${this.account}.dkr.ecr.${this.region}.amazonaws.com || true`,
      'cd /opt/lymmholidaylets && docker compose up -d',

      // Configure CloudWatch agent to forward container logs.
      'dnf install -y amazon-cloudwatch-agent',
      'cat > /opt/aws/amazon-cloudwatch-agent/bin/config.json << \'CW\'',
      '{',
      '  "logs": {',
      '    "logs_collected": {',
      '      "files": {',
      '        "collect_list": [',
      '          {',
      '            "file_path": "/var/lib/docker/containers/**/*-json.log",',
      '            "log_group_name": "/lymmholidaylets/' + envName + '/api",',
      '            "log_stream_name": "{instance_id}",',
      '            "timestamp_format": "%Y-%m-%dT%H:%M:%S.%fZ"',
      '          }',
      '        ]',
      '      }',
      '    }',
      '  }',
      '}',
      'CW',
      '/opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-ctl -a fetch-config -m ec2 -s -c file:/opt/aws/amazon-cloudwatch-agent/bin/config.json',
    );

    // ── EC2 Instance ─────────────────────────────────────────────────────────
    // Amazon Linux 2023 (x86_64) – latest SSM-managed AMI.
    const ami = ec2.MachineImage.latestAmazonLinux2023({
      cpuType: ec2.AmazonLinuxCpuType.X86_64,
    });

    const instance = new ec2.Instance(this, 'ApiInstance', {
      instanceName: `lymmholidaylets-${envName}-api`,
      vpc,
      vpcSubnets: { subnetType: ec2.SubnetType.PRIVATE_WITH_EGRESS },
      instanceType: ec2.InstanceType.of(ec2.InstanceClass.T3, ec2.InstanceSize.SMALL),
      machineImage: ami,
      securityGroup: ec2Sg,
      role: instanceRole,
      userData,
      blockDevices: [
        {
          deviceName: '/dev/xvda',
          volume: ec2.BlockDeviceVolume.ebs(30, {
            volumeType: ec2.EbsDeviceVolumeType.GP3,
            encrypted: true,
            deleteOnTermination: true,
          }),
        },
      ],
      // Enable detailed monitoring for CloudWatch metrics.
      detailedMonitoring: true,
    });

    // ── Application Load Balancer ─────────────────────────────────────────────
    const alb = new elasticloadbalancingv2.ApplicationLoadBalancer(this, 'Alb', {
      loadBalancerName: `lymmholidaylets-${envName}-alb`,
      vpc,
      internetFacing: true,
      securityGroup: albSg,
    });

    // Target group pointing at the EC2 instance on port 8080.
    const targetGroup = new elasticloadbalancingv2.ApplicationTargetGroup(this, 'ApiTg', {
      targetGroupName: `lymmholidaylets-${envName}-api-tg`,
      vpc,
      port: 8080,
      protocol: elasticloadbalancingv2.ApplicationProtocol.HTTP,
      targets: [new elasticloadbalancingv2targets.InstanceTarget(instance, 8080)],
      healthCheck: {
        path: '/health',
        healthyHttpCodes: '200',
        interval: cdk.Duration.seconds(30),
        timeout: cdk.Duration.seconds(10),
        healthyThresholdCount: 2,
        unhealthyThresholdCount: 5,
      },
    });

    // HTTP listener → redirect to HTTPS (or forward directly if no cert).
    const acmCertArn = process.env.ACM_CERTIFICATE_ARN;

    if (acmCertArn) {
      // HTTPS listener (production path).
      const certificate = acm.Certificate.fromCertificateArn(this, 'Cert', acmCertArn);

      alb.addListener('HttpsListener', {
        port: 443,
        protocol: elasticloadbalancingv2.ApplicationProtocol.HTTPS,
        certificates: [certificate],
        defaultTargetGroups: [targetGroup],
      });

      // HTTP → HTTPS redirect.
      alb.addListener('HttpListener', {
        port: 80,
        defaultAction: elasticloadbalancingv2.ListenerAction.redirect({
          protocol: 'HTTPS',
          port: '443',
          permanent: true,
        }),
      });
    } else {
      // Dev/staging: plain HTTP forwarding (no cert required).
      alb.addListener('HttpListener', {
        port: 80,
        defaultTargetGroups: [targetGroup],
      });
    }

    // ── Outputs ──────────────────────────────────────────────────────────────
    new cdk.CfnOutput(this, 'AlbDnsName', {
      description: 'ALB DNS name – point your domain CNAME here',
      value: alb.loadBalancerDnsName,
    });

    new cdk.CfnOutput(this, 'InstanceId', {
      description: 'EC2 instance ID (use SSM Session Manager to connect)',
      value: instance.instanceId,
    });

    new cdk.CfnOutput(this, 'InstancePrivateIp', {
      description: 'Private IP of the EC2 instance',
      value: instance.instancePrivateIp,
    });

    // ── SSM Parameter stubs (helpful reminder of required secrets) ────────────
    // These are NOT created with real values – you must populate them manually.
    // They serve as documentation of what the instance expects at boot time.
    new cdk.CfnOutput(this, 'RequiredSsmParameters', {
      description: 'Create these SSM SecureString parameters before deploying',
      value: [
        `/lymmholidaylets/${envName}/db/password`,
        `/lymmholidaylets/${envName}/stripe/webhookKey`,
        `/lymmholidaylets/${envName}/sendgrid/apiKey`,
        `/lymmholidaylets/${envName}/logzio/token`,
      ].join(', '),
    });
  }
}
