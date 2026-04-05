# LymmHolidayLets

## Overview
LymmHolidayLets is a modern .NET 10.0 solution for managing holiday property bookings, payments, reviews, and more. It features a robust API, GraphQL endpoints, database migration scripts, and a Next.js UI frontend.

## Solution Structure
- **src/LymmHolidayLets.Api**: ASP.NET Core Web API, GraphQL, controllers, model binding, and service registration.
- **src/LymmHolidayLets.Application**: Application layer, business logic, service orchestration, and interfaces.
- **src/LymmHolidayLets.Application.Model**: DTOs and result types used by the application layer.
- **src/LymmHolidayLets.Domain**: Domain models, interfaces, and core business rules.
- **src/LymmHolidayLets.Infrastructure**: Data access, repositories, logging, email services, and dependency injection.
- **src/LymmHolidayLets.Contracts**: Shared contracts and interfaces.
- **src/LymmHolidayLets.Applications/CalendarImporter**: Background worker for importing calendar data from external sources (iCal).
- **src/LymmHolidayLets.EmailWorker**: Background worker for processing email notifications via RabbitMQ.
- **src/LymmHolidayLets.UnitTests**: Fast unit tests (controllers, application services, API helpers).
- **src/LymmHolidayLets.IntegrationTests**: HTTP-level tests against the API host (`WebApplicationFactory`).
- **db/**: Database migration scripts, DACPAC, and setup files.
- **src/lymmholidaylets.ui/**: Next.js frontend for property listings and booking.
- **infra/**: AWS CDK infrastructure as code (TypeScript).

## Key Features
- Property and calendar management
- Stripe payment integration
- Review and FAQ system
- Email notifications via RabbitMQ message queue (SendGrid, SMTP)
- Background workers for email processing and calendar imports (iCal)
- REST and GraphQL API endpoints (HotChocolate)
- FluentValidation for input validation
- Dual data access pattern (EF Core + Dapper with stored procedures)
- Logging via Serilog and Logz.io
- Docker support for API, UI, database, RabbitMQ, and workers
- Clean/Hexagonal architecture with CQRS

## Getting Started

### Prerequisites
- .NET 10.0 SDK
- Node.js 18+ (for UI)
- Docker & Docker Compose (recommended)
- SQL Server (or use Docker)

### Option 1: Run Everything with Docker Compose (Recommended)
This starts the API, UI, SQL Server database, RabbitMQ, and background workers (EmailWorker, CalendarImporter):
```sh
docker-compose up
```
Services will be available at:
- API: http://localhost:8080
- UI: http://localhost:3003
- GraphQL Playground: http://localhost:8080/graphql
- RabbitMQ Management: http://localhost:15672 (guest/guest)
- CalendarImporter: Runs in background (check logs with `docker-compose logs -f lymmholidaylets-calendarimporter`)

### Option 2: Run Locally for Development

1. **Build the Solution**
   ```sh
   dotnet build lymmholidaylets.slnx
   ```

2. **Set up the Database**
   - Use Docker: `docker-compose up mssql`
   - Or manually configure SQL Server and run scripts in `db/`

3. **Run the API**
   ```sh
   dotnet run --project src/LymmHolidayLets.Api/LymmHolidayLets.Api.csproj
   ```

4. **Run the UI**
   ```sh
   cd src/lymmholidaylets.ui
   npm install
   npm run dev
   ```

5. **(Optional) Run Background Workers**
   ```sh
   # Email worker
   dotnet run --project src/LymmHolidayLets.EmailWorker/LymmHolidayLets.EmailWorker.csproj
   
   # Calendar importer
   dotnet run --project src/LymmHolidayLets.Applications/CalendarImporter/CalendarImporter.csproj
   ```

## Testing
Run all tests:
```sh
dotnet test lymmholidaylets.slnx
```

Run a specific test project:
```sh
dotnet test src/LymmHolidayLets.UnitTests
dotnet test src/LymmHolidayLets.IntegrationTests
```

Run a single test by name:
```sh
dotnet test lymmholidaylets.slnx --filter "FullyQualifiedName~CheckoutServiceTests"
```

## Architecture

The project follows Clean/Hexagonal architecture with CQRS patterns:

```
Api → Application → Domain ← Infrastructure
```

**Key Principles:**
- **Dependency flow**: Api depends on Application, Application depends on Domain, Infrastructure implements Domain interfaces
- **Dual data access**: EF Core for GraphQL queries (returns `IQueryable`), Dapper for everything else (calls stored procedures)
- **CQRS**: Commands (writes) and Queries (reads) are separated in the Application layer
- **GraphQL**: Served via HotChocolate at `/graphql` for flexible data fetching
- **Validation**: FluentValidation only (no data annotations)
- **API Responses**: All REST endpoints return `ApiResponse<T>` wrapper

**Background Processing:**
- RabbitMQ message queue for asynchronous email sending
- CalendarImporter worker for syncing external iCal feeds
- EmailWorker for processing email notifications from the queue

## Configuration
- **API settings**: `src/LymmHolidayLets.Api/appsettings.json`
- **UI settings**: `src/lymmholidaylets.ui/`
- **Environment variables** (for secrets):
  - `ConnectionStrings__LymmHolidayLets`: SQL Server connection string
  - `SendGrid__ApiKey`: SendGrid API key for email
  - `RabbitMQ__Host`, `RabbitMQ__Username`, `RabbitMQ__Password`: Message queue configuration
  - Stripe keys (for payment processing)
  - Logz.io tokens (for logging)

See [AGENTS.md](AGENTS.md) and [CLAUDE.md](CLAUDE.md) for detailed development guidelines.

## License
MIT License

## Maintainers
- LymmHolidayLets Team

