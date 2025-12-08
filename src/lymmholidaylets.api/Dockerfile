FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy repository (use .dockerignore to exclude bin/ obj/ .git)
COPY . .

# Restore using explicit project path
RUN dotnet restore src/lymmholidaylets.api/lymmholidaylets.api.csproj

# Publish using explicit project path
RUN dotnet publish src/lymmholidaylets.api/lymmholidaylets.api.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "lymmholidaylets.api.dll"]