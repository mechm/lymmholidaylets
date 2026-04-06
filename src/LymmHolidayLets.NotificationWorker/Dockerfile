FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore src/LymmHolidayLets.NotificationWorker/LymmHolidayLets.NotificationWorker.csproj

RUN dotnet publish src/LymmHolidayLets.NotificationWorker/LymmHolidayLets.NotificationWorker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LymmHolidayLets.NotificationWorker.dll"]
