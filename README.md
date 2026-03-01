# LymmHolidayLets

## Overview
LymmHolidayLets is a modern .NET 10.0 solution for managing holiday property bookings, payments, reviews, and more. It features a robust API, GraphQL endpoints, database migration scripts, and a Next.js UI frontend.

## Solution Structure
- **src/LymmHolidayLets.Api**: ASP.NET Core Web API, GraphQL, controllers, model binding, and service registration.
- **src/LymmHolidayLets.Application**: Application layer, business logic, service orchestration, and interfaces.
- **src/LymmHolidayLets.Domain**: Domain models, interfaces, and core business rules.
- **src/LymmHolidayLets.Infrastructure**: Data access, repositories, logging, email services, and dependency injection.
- **src/LymmHolidayLets.Api.Tests**: Unit and integration tests for the API and services.
- **db/**: Database migration scripts, DACPAC, and setup files.
- **lymmholidaylets.ui/**: Next.js frontend for property listings and booking.

## Key Features
- Property and calendar management
- Stripe payment integration
- Review and FAQ system
- Email notifications (SendGrid, SMTP)
- GraphQL API endpoints
- FluentValidation for input validation
- Logging via Serilog and Logz.io
- Docker support for API, UI, and database

## Getting Started
1. **Prerequisites**
   - .NET 10.0 SDK
   - Node.js (for UI)
   - Docker (optional)
2. **Build the Solution**
   ```sh
   dotnet build lymmholidaylets.slnx
   ```
3. **Run the API**
   ```sh
   dotnet run --project src/LymmHolidayLets.Api/LymmHolidayLets.Api.csproj
   ```
4. **Run the UI**
   ```sh
   cd lymmholidaylets.ui
   npm install
   npm run dev
   ```
5. **Database Setup**
   - Use scripts in `db/` or run via Docker Compose.

## Testing
Run all tests:
```sh
dotnet test lymmholidaylets.slnx
```

## Configuration
- API settings: `src/LymmHolidayLets.Api/appsettings.json`
- UI settings: `lymmholidaylets.ui/`
- Environment variables for secrets (Stripe, SendGrid, Logz.io)

## Contributing
Pull requests and issues are welcome. Please follow the code style and add tests for new features.

## License
MIT License

## Maintainers
- LymmHolidayLets Team

