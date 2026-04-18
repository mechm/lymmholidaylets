# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/LymmHolidayLets.NotificationWorker/LymmHolidayLets.NotificationWorker.csproj", "src/LymmHolidayLets.NotificationWorker/"]
COPY ["src/LymmHolidayLets.Contracts/LymmHolidayLets.Contracts.csproj", "src/LymmHolidayLets.Contracts/"]
COPY ["src/LymmHolidayLets.Application/LymmHolidayLets.Application.csproj", "src/LymmHolidayLets.Application/"]
COPY ["src/LymmHolidayLets.Application.Model/LymmHolidayLets.Application.Model.csproj", "src/LymmHolidayLets.Application.Model/"]
COPY ["src/LymmHolidayLets.Infrastructure/LymmHolidayLets.Infrastructure.csproj", "src/LymmHolidayLets.Infrastructure/"]
COPY ["src/LymmHolidayLets.Domain/LymmHolidayLets.Domain.csproj", "src/LymmHolidayLets.Domain/"]
RUN dotnet restore "src/LymmHolidayLets.NotificationWorker/LymmHolidayLets.NotificationWorker.csproj"

# Copy everything else and publish
COPY . .
RUN dotnet publish "src/LymmHolidayLets.NotificationWorker/LymmHolidayLets.NotificationWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LymmHolidayLets.NotificationWorker.dll"]
