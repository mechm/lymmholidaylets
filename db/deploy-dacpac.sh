#!/bin/bash

# --- VARIABLES ---
LOG_FILE="/tmp/sql_startup_debug.log"
DACPAC_PATH="/tmp/LymmHolidayLets.Db.dacpac" # Assumes DACPAC is copied here in the Dockerfile
DB_NAME="LymmHolidayLets"                # Target database name
SQLPACKAGE="/opt/sqlpackage/sqlpackage" # Path to the SqlPackage CLI
TRIES=50
i=0

# --- INITIAL SETUP ---
# Start SQL Server in the background
echo "Starting SQL Server in background..."
/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server to start (TRIES: $TRIES)..."

# --- MAIN LOOP: WAIT FOR SQL SERVER ---
# We use sqlcmd to check if the server is ready to accept connections.
while [[ $i -lt $TRIES ]]; do

    echo "--- Attempt $((i+1)) / $TRIES ---"
    
    # Use sqlcmd to attempt a connection (using -C for TrustServerCertificate=True)
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" -C -h -1 > "$LOG_FILE" 2>&1
    
    SQLCMD_EXIT_CODE=$?
    
    if [ $SQLCMD_EXIT_CODE -eq 0 ]; then
        echo "SQL Server is ready. Connection Successful."
        
        # --- DEPLOY THE DACPAC ---
        echo "Deploying DACPAC: $DB_NAME from $DACPAC_PATH"
        
        # The SqlPackage Publish command to deploy the DACPAC
        # Note: /ttsc:True (Trust Server Certificate) is needed for the local Docker connection
        "$SQLPACKAGE" /a:Publish \
            /sf:"$DACPAC_PATH" \
            /tsn:localhost \
            /tdn:"$DB_NAME" \
            /tu:sa /tp:"$MSSQL_SA_PASSWORD" \
            /ttsc:True

        DEPLOY_EXIT_CODE=$?
        
        if [ $DEPLOY_EXIT_CODE -eq 0 ]; then
            echo "Database initialization via DACPAC completed successfully."
            break
        else
            echo "FATAL ERROR: DACPAC deployment failed with exit code $DEPLOY_EXIT_CODE."
            exit 1
        fi
        
    else
        # If the connection failed, print the error log and wait
        echo "Connection failed (Exit Code $SQLCMD_EXIT_CODE). Printing last error:"
        cat "$LOG_FILE"
        echo "------------------------------------------"
    fi
    
    sleep 2s
    i=$((i+1))
done

# --- FAILURE CONDITION ---
if [ $i -eq $TRIES ]; then
    echo "FATAL ERROR: SQL Server took too long to start or failed to connect."
    exit 1
fi

# --- KEEP CONTAINER ALIVE ---
# Wait for the background SQL Server process to keep the container running
wait