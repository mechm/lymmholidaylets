#!/bin/bash

# --- INITIAL SETUP ---
LOG_FILE="/tmp/sql_startup_debug.log"
TRIES=50
i=0

# Start SQL Server in the background
echo "Starting SQL Server in background..."
/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server to start (TRIES: $TRIES)..."

# --- MAIN LOOP ---
while [[ $i -lt $TRIES ]]; do

    echo "--- Attempt $((i+1)) / $TRIES ---"
    
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" -C -h -1 > "$LOG_FILE" 2>&1
    
    SQLCMD_EXIT_CODE=$? # Store the exit code of the last command
    
    if [ $SQLCMD_EXIT_CODE -eq 0 ]; then
        echo "SQL Server is ready (Exit Code 0). Connection Successful."
        
        # Execute the main SQL script (use the same flag for the setup query)
        echo "Executing setup.sql..."
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i setup.sql
        
        echo "Database initialization complete."
        break
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

# Keep the container running
wait