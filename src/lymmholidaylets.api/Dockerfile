FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY src/lymmholidaylets.api/lymmholidaylets.api.csproj lymmholidaylets.api/
RUN dotnet restore ./lymmholidaylets.api/lymmholidaylets.api.csproj

# Copy everything else and build
WORKDIR /src/lymmholidaylets.api
COPY src/lymmholidaylets.api/. .
RUN dotnet publish lymmholidaylets.api.csproj -c Release -o /app/publish

# Use the official .NET 10 ASP.NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "lymmholidaylets.api.dll"]