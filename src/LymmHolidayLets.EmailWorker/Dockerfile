FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore src/LymmHolidayLets.EmailWorker/LymmHolidayLets.EmailWorker.csproj

RUN dotnet publish src/LymmHolidayLets.EmailWorker/LymmHolidayLets.EmailWorker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LymmHolidayLets.EmailWorker.dll"]
