# Lymm Holiday Lets Solution

This repository contains the source code and infrastructure for the Lymm Holiday Lets platform, including API, database, application logic, domain models, and UI.

## Solution Structure

```
lymmholidaylets/
├── db/                  # Database scripts, Dockerfile, and DACPAC
├── src/                 # Source code for all projects
│   ├── LymmHolidayLets.Api/            # ASP.NET Core Web API
│   ├── LymmHolidayLets.Api.Tests/      # API unit/integration tests
│   ├── LymmHolidayLets.Application/    # Application layer (CQRS, services)
│   ├── LymmHolidayLets.Application.Model/ # Application models
│   ├── LymmHolidayLets.Domain/         # Domain models and interfaces
│   ├── LymmHolidayLets.Infrastructure/ # Infrastructure (EF, Dapper, Email, Logging)
│   └── lymmholidaylets.ui/             # Next.js frontend
├── docker-compose.yml   # Docker Compose for multi-container orchestration
├── global.json          # .NET SDK versioning
├── lymmholidaylets.slnx # Solution file
└── README.md            # This file
```

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js (for UI)](https://nodejs.org/)
- [Docker](https://www.docker.com/)
- SQL Server (local or Docker)

### Build and Run (Development)

1. **Clone the repository:**
   ```sh
   git clone <repo-url>
   cd lymmholidaylets
   ```

2. **Build and run the API:**
   ```sh
   cd src/LymmHolidayLets.Api
   dotnet build
   dotnet run
   ```

3. **Run the UI:**
   ```sh
   cd src/lymmholidaylets.ui
   npm install
   npm run dev
   ```

4. **Run the database (Docker):**
   ```sh
   docker-compose up db
   ```

### Running Tests

```sh
cd src/LymmHolidayLets.Api.Tests
 dotnet test
```

## Key Features
- ASP.NET Core Web API with GraphQL
- Entity Framework Core and Dapper for data access
- Modular application and domain layers
- Email sending with SMTP
- Rate limiting and security best practices
- Next.js frontend
- Dockerized database and services

## Configuration
- API settings: `src/LymmHolidayLets.Api/appsettings.json`
- SMTP/email: `Smtp` section in configuration
- ReCaptcha: `Keys` section in configuration

## Contributing
Pull requests are welcome! Please open issues for bugs or feature requests.

## License
MIT License



//TODO
Share calendar between properties
Book dates that are blocked on airbnb for example
Local businesses page and discount

