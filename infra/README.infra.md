# Infrastructure – AWS CDK (TypeScript)

This folder contains the AWS CDK stack that deploys the **LymmHolidayLets API** onto an EC2 instance behind an Application Load Balancer.

## Architecture

```
Internet
   │
   ▼
[ALB – public subnets]   ← HTTP(80) redirect to HTTPS(443)
   │                       HTTPS(443) → forwarded if ACM cert supplied
   ▼
[EC2 t3.small – private subnet]
   └─ Docker container: LymmHolidayLets.Api (port 8080)
      ├─ Secrets pulled from SSM Parameter Store at boot
      └─ Logs shipped to CloudWatch + Logz.io
```

- **VPC**: 2 AZs, public + private subnets, 1 NAT Gateway  
- **EC2**: Amazon Linux 2023, Docker + Docker Compose v2 installed via UserData  
- **ALB**: internet-facing, health-checks `/health` on port 8080  
- **IAM role**: SSM Session Manager (no SSH key needed), ECR read, CloudWatch  
- **Secrets**: stored in SSM Parameter Store as `SecureString`

---

## Prerequisites

| Tool | Version |
|------|---------|
| Node.js | 18+ |
| AWS CDK CLI | v2 (`npm i -g aws-cdk`) |
| AWS CLI | v2 |
| Docker | any recent version (for `cdk bootstrap` assets) |

Configure AWS credentials:
```bash
aws configure          # or use AWS_PROFILE / environment variables
```

---

## Step 1 – Bootstrap (once per account/region)

```bash
cdk bootstrap aws://<ACCOUNT_ID>/eu-west-2
```

---

## Step 2 – Create SSM secrets

Run these commands (replace placeholder values with real secrets):

```bash
ENV=dev    # or staging / prod

aws ssm put-parameter \
  --name "/lymmholidaylets/$ENV/db/password" \
  --value "YourStrong!Password123" \
  --type SecureString --overwrite

aws ssm put-parameter \
  --name "/lymmholidaylets/$ENV/stripe/webhookKey" \
  --value "whsec_..." \
  --type SecureString --overwrite

aws ssm put-parameter \
  --name "/lymmholidaylets/$ENV/sendgrid/apiKey" \
  --value "SG...." \
  --type SecureString --overwrite

aws ssm put-parameter \
  --name "/lymmholidaylets/$ENV/logzio/token" \
  --value "your-logzio-token" \
  --type SecureString --overwrite
```

---

## Step 3 – Push the Docker image to ECR

```bash
REGION=eu-west-2
ACCOUNT=$(aws sts get-caller-identity --query Account --output text)
REPO=lymmholidaylets-api

# Create the repository (once)
aws ecr create-repository --repository-name $REPO --region $REGION

# Authenticate Docker to ECR
aws ecr get-login-password --region $REGION \
  | docker login --username AWS --password-stdin $ACCOUNT.dkr.ecr.$REGION.amazonaws.com

# Build and push (from the repository root)
cd ..
docker build -f src/LymmHolidayLets.Api/Dockerfile -t $REPO:latest .
docker tag  $REPO:latest $ACCOUNT.dkr.ecr.$REGION.amazonaws.com/$REPO:latest
docker push $ACCOUNT.dkr.ecr.$REGION.amazonaws.com/$REPO:latest
```

Set the `ECR_IMAGE` environment variable in the `.env` file on the instance, e.g.:
```
ECR_IMAGE=123456789.dkr.ecr.eu-west-2.amazonaws.com/lymmholidaylets-api:latest
```

---

## Step 4 – (Optional) HTTPS / ACM Certificate

If you have a domain in Route 53 or an existing ACM certificate:

```bash
export ACM_CERTIFICATE_ARN=arn:aws:acm:eu-west-2:<ACCOUNT>:certificate/<UUID>
```

Without this variable, the ALB listens on HTTP port 80 only (suitable for dev).

---

## Step 5 – Deploy

```bash
cd infra

# Install dependencies
npm install

# Preview changes
npm run synth         # generates CloudFormation template
npm run diff          # compare with current deployed state

# Deploy (dev environment)
npm run deploy                     # defaults to env=dev
cdk deploy -c env=prod             # production environment

# Destroy (tear down all resources)
npm run destroy
```

---

## Connecting to the instance (no SSH required)

Use SSM Session Manager:

```bash
aws ssm start-session --target <InstanceId>
```

The `InstanceId` is printed as a CDK output after deployment.

---

## CloudWatch Logs

API logs are streamed to the log group:
```
/lymmholidaylets/<env>/api
```

---

## Environment variables set on the EC2 instance

| Variable | Source |
|----------|--------|
| `ASPNETCORE_ENVIRONMENT` | Hardcoded: `Production` |
| `ASPNETCORE_URLS` | Hardcoded: `http://+:8080` |
| `DB_PASSWORD` | SSM: `…/db/password` |
| `STRIPE_WEBHOOK_KEY` | SSM: `…/stripe/webhookKey` |
| `SENDGRID_API_KEY` | SSM: `…/sendgrid/apiKey` |
| `LOGZIO_TOKEN` | SSM: `…/logzio/token` |

The `ConnectionStrings__LymmHolidayLets` connection string should be supplied via a separate SSM parameter or added to the `docker-compose.yml` environment section using the fetched `DB_PASSWORD`.
