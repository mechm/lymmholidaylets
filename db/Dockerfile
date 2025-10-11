# Use the latest official SQL Server on Linux image
FROM mcr.microsoft.com/mssql/server:2022-latest

# Switch to root to ensure installation and permission changes work
USER root

# Install dependencies (only for setting permissions/line endings)
RUN apt-get update && \
    apt-get install -y dos2unix procps && \
    rm -rf /var/lib/apt/lists/*

# Create a directory for our scripts
WORKDIR /usr/src/app

# Copy the custom scripts and SQL file
COPY ./entrypoint.sh ./
COPY ./setup.sql ./setup.sql

# Convert line endings (CRLF to LF) for Linux execution and set permissions
RUN dos2unix ./entrypoint.sh && \
    chmod +x ./entrypoint.sh

# Switch back to the default mssql user
USER mssql

# Override the default command to run our custom entrypoint script
CMD ["/bin/bash", "./entrypoint.sh"]