# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["src/LymmHolidayLets.CalendarMaintenanceWorker/LymmHolidayLets.CalendarMaintenanceWorker.csproj", "src/LymmHolidayLets.CalendarMaintenanceWorker/"]
RUN dotnet restore "src/LymmHolidayLets.CalendarMaintenanceWorker/LymmHolidayLets.CalendarMaintenanceWorker.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/LymmHolidayLets.CalendarMaintenanceWorker"
RUN dotnet build "LymmHolidayLets.CalendarMaintenanceWorker.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "LymmHolidayLets.CalendarMaintenanceWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LymmHolidayLets.CalendarMaintenanceWorker.dll"]
