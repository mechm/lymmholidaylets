# Copilot Instructions — LymmHolidayLets

Holiday lettings management app. .NET 10 backend (REST + GraphQL) with a Next.js 15 frontend.

## Commands

```sh
# Build
dotnet build lymmholidaylets.slnx

# Run all tests
dotnet test lymmholidaylets.slnx

# Run a single test by name
dotnet test lymmholidaylets.slnx --filter "FullyQualifiedName~CheckoutServiceTests"

# Run a specific test project
dotnet test src/LymmHolidayLets.UnitTests
dotnet test src/LymmHolidayLets.IntegrationTests

# Run the API directly
dotnet run --project src/LymmHolidayLets.Api/LymmHolidayLets.Api.csproj

# Run everything (API + UI + DB)
docker-compose up

# Run the UI
cd src/lymmholidaylets.ui && npm install && npm run dev
```

## Architecture

Clean/Hexagonal architecture with CQRS. Dependency flow:

```
Api → Application → Domain ← Infrastructure
```

- **`LymmHolidayLets.Api`** — Controllers, GraphQL (HotChocolate), FluentValidation validators, rate limiting. No business logic here.
- **`LymmHolidayLets.Application`** — Commands, queries, and services (orchestration). Split into:
  - `Command/` — write-side (e.g. `BookingCommand.cs`)
  - `Query/` — read-side (e.g. `PropertyQuery.cs`)
  - `Service/` — orchestration services (e.g. `CheckoutService.cs`)
  - `Interface/` — interfaces for all of the above
- **`LymmHolidayLets.Application.Model`** — DTOs and result types used by the application layer. Lives in a separate project to avoid circular references.
- **`LymmHolidayLets.Domain`** — Entities, repository interfaces, read models, domain exceptions. Never depends on infrastructure.
- **`LymmHolidayLets.Infrastructure`** — EF Core (`AppDbContext`) + Dapper repositories, emailer, logging. Only place that touches the DB.
- **`LymmHolidayLets.Applications`** — Background workers (e.g. iCal importer).

### Dual data access pattern

The codebase uses **two** data access strategies simultaneously:

- **Dapper** (`Infrastructure/Repository/Dapper/`) — used for most reads and writes via stored procedures. All Dapper repos extend `RepositoryBase<T>` and call stored procedures by name.
- **EF Core** (`Infrastructure/Repository/EF/`, `AppDbContext.cs`) — used selectively for entities exposed via GraphQL (Calendar, Property, Page). EF repositories return `IQueryable<T>` so HotChocolate can compose queries.

When adding a new read for GraphQL, use EF. For everything else, use Dapper with a stored procedure.

### GraphQL surface

GraphQL is served via HotChocolate at `/graphql`. Queries are in `Api/GraphQL/Queries/`. The EF-based `IQueryable` repositories are the data source. REST controllers and GraphQL queries can coexist for the same entity.

## Key Conventions

### API responses
All REST controller actions must return `ApiResponse<T>` from `Api/Models/ApiResponse.cs`:
```csharp
return Ok(ApiResponse<T>.SuccessResult(data));
return BadRequest(ApiResponse<object>.FailureResult("message", errors));
```

### Validation
FluentValidation only — never data annotations. Validators live in `Api/Validators/` and are registered there. The application layer also has `Application/Validation/` for shared validation helpers (e.g. `DateValidation.cs`).

### Controllers
Controllers are thin: validate input via FluentValidation (auto-wired), call a service/command/query, wrap the result in `ApiResponse<T>`, and return. No business logic.

### Services vs Commands/Queries
- **Commands** (`IBookingCommand`, etc.) — simple write operations delegating to a single repository.
- **Queries** (`IPropertyQuery`, etc.) — simple read operations delegating to a data adapter or EF repository.
- **Services** (`ICheckoutService`, etc.) — orchestration that coordinates multiple commands/queries/external calls (Stripe, email). These are where complex logic lives.

### Domain exceptions
Infrastructure-level data errors throw `DataAccessException` (in `Infrastructure/Exception/`). Domain-level client errors throw `ClientSideException` (in `Domain/Exception/`).

### Logging
Serilog throughout. Use structured logging (`logger.LogInformation("Message {Prop}", value)`). Logz.io is the log sink in production.

### Database
SQL Server. Data changes go through stored procedures (Dapper path). Schema changes must be reflected in `db/` scripts and the `.sqlproj`. EF Core is **not** used for migrations — schema is managed via DACPAC.

## Testing

- **Unit tests** (`LymmHolidayLets.UnitTests`) — test services and application logic. Use `Moq` for mocks, `FluentAssertions` for assertions. Mock at the interface boundary (e.g. `ICheckoutQuery`, `IStripeService`). Use a private `CreateSut()` factory pattern.
- **Integration tests** (`LymmHolidayLets.IntegrationTests`) — HTTP-level tests using `WebApplicationFactory<Program>`. `ApiFactory` (in `IntegrationTests/Infrastructure/`) replaces all services with mocks. Tests verify routing, middleware, model binding, and `ApiResponse<T>` shape — not business logic.

When adding a new feature, add unit tests for the service and integration tests for the endpoint.

## External Integrations

- **Stripe** — payments and webhooks. `IStripeService` abstracts the SDK. Webhook validation happens in `StripeWebHookController`.
- **SMTP2GO / MailKit** — email via `Emailer/` in Infrastructure.
- **Logz.io** — structured log shipping.
- **reCAPTCHA** — validated via `IRecaptchaValidationService` before processing user-submitted forms.

## Infrastructure (AWS)

AWS CDK app lives in `infra/` (TypeScript). Deploy and manage separately from the application code.
