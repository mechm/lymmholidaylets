# Use the latest official SQL Server on Linux image
FROM mcr.microsoft.com/mssql/server:2022-latest

# Switch to root to ensure installation and permission changes work
USER root

# Install dependencies (only for setting permissions/line endings)
#RUN apt-get update && \
    #apt-get install -y dos2unix procps && \
    #rm -rf /var/lib/apt/lists/*
#
## Create a directory for our scripts
#WORKDIR /usr/src/app
#
## Copy the custom scripts and SQL file
#COPY ./entrypoint.sh ./
#COPY ./setup.sql ./setup.sql
#
## Convert line endings (CRLF to LF) for Linux execution and set permissions
#RUN dos2unix ./entrypoint.sh && \
    #chmod +x ./entrypoint.sh
#
## Switch back to the default mssql user
#USER mssql
#
## Override the default command to run our custom entrypoint script
#CMD ["/bin/bash", "./entrypoint.sh"]


# Install dependencies for running the wait script and necessary tools for SqlPackage
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    wget \
    unzip \
    curl \
    dos2unix \
    procps \
    libunwind8 \
    libicu70 \
    && rm -rf /var/lib/apt/lists/*

# Install SQLPackage for Linux (FIXED DOWNLOAD VIA CURL/TAR)
RUN wget -progress=bar:force -q -O sqlpackage.zip https://aka.ms/sqlpackage-linux \
    && unzip -qq sqlpackage.zip -d /opt/sqlpackage \
    && chmod +x /opt/sqlpackage/sqlpackage \
    && chown -R mssql /opt/sqlpackage \
    && mkdir /tmp/db \
    && chown -R mssql /tmp/db

# Create a directory for our deployment scripts
WORKDIR /usr/src/app
#
## --- 1. COPY DACPAC AND DEPLOYMENT SCRIPT ---
## Assume your compiled DACPAC is named 'LymmHolidayLets.Db.dacpac'
COPY ./lymmholidaylets.db/Snapshots/LymmHolidayLets.Db.dacpac /tmp/LymmHolidayLets.Db.dacpac
COPY ./deploy-dacpac.sh ./deploy-dacpac.sh
#
## Convert line endings and set permissions for the deployment script
RUN chmod +x ./deploy-dacpac.sh
#
## Switch back to the default mssql user
USER mssql
#
## Override the default command to run our custom deploy-dacpac.sh
CMD ["/bin/bash", "./deploy-dacpac.sh"]