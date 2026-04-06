# AGENTS.md — LymmHolidayLets

Holiday lettings management app. .NET 10 backend (REST + GraphQL) with a Next.js 15 frontend.

## Commands

```sh
# Build (run this after making changes to verify compilation)
dotnet build lymmholidaylets.slnx

# Run all tests
dotnet test lymmholidaylets.slnx

# Run a single test by name
dotnet test lymmholidaylets.slnx --filter "FullyQualifiedName~CheckoutServiceTests"

# Run a specific project's tests
dotnet test src/LymmHolidayLets.UnitTests
dotnet test src/LymmHolidayLets.IntegrationTests

# Run the API locally
dotnet run --project src/LymmHolidayLets.Api/LymmHolidayLets.Api.csproj

# Start everything (API + UI + SQL Server)
docker-compose up

# UI dev server
cd src/lymmholidaylets.ui && npm install && npm run dev
```

## Verification

After making any code change:
1. Run `dotnet build lymmholidaylets.slnx` — fix all compiler errors and warnings before proceeding.
2. Run `dotnet test lymmholidaylets.slnx` — all tests must pass.
3. For new features or bug fixes, add tests before marking the task done.

## Architecture

Dependency flow (arrows = allowed dependency direction):

```
Api → Application → Domain ← Infrastructure
```

| Project | Purpose |
|---|---|
| `LymmHolidayLets.Api` | Controllers, GraphQL, validators, middleware |
| `LymmHolidayLets.Application` | Commands, queries, orchestration services |
| `LymmHolidayLets.Application.Model` | DTOs and result types (separate to avoid circular refs) |
| `LymmHolidayLets.Domain` | Entities, repository interfaces, read models, domain exceptions |
| `LymmHolidayLets.Infrastructure` | EF Core + Dapper repos, emailer, logging |
| `LymmHolidayLets.Applications` | Background workers (e.g. iCal importer) |

**Rules:**
- Never reference `Infrastructure` from `Application` or `Domain`.
- Never put business logic in controllers — it belongs in `Application/Service/`.
- Never access `AppDbContext` outside of `Infrastructure`.

## Data Access

Two strategies are used together:

- **Dapper** (`Infrastructure/Repository/Dapper/`) — default for reads and writes. Always calls stored procedures by name. Extend `RepositoryBase<T>`.
- **EF Core** (`Infrastructure/Repository/EF/`, `AppDbContext.cs`) — used only for entities exposed via GraphQL (Calendar, Property, Page). Return `IQueryable<T>`.

When writing a new read query for GraphQL → use EF. For everything else → use Dapper + a stored procedure.

## Key Conventions

### All REST responses use `ApiResponse<T>`
```csharp
return Ok(ApiResponse<T>.SuccessResult(data));
return BadRequest(ApiResponse<object>.FailureResult("message", errors));
```

### Validation
Use FluentValidation only — never `[Required]` or other data annotations. Validators go in `Api/Validators/`.

### Application layer structure
- `Command/` — write operations, one class per aggregate, delegates to a single repository.
- `Query/` — read operations, delegates to a data adapter or EF repository.
- `Service/` — orchestration across multiple commands/queries/external services.
- `Interface/` — an interface for every command, query, and service.

### Exceptions
- `DataAccessException` (`Infrastructure/Exception/`) — for DB errors.
- `ClientSideException` (`Domain/Exception/`) — for domain rule violations.

### Logging
Serilog structured logging throughout:
```csharp
logger.LogInformation("Checkout session created for PropertyId={PropertyId}", propertyId);
```

## Testing Patterns

### Unit tests (`LymmHolidayLets.UnitTests`)
- xUnit + Moq + FluentAssertions.
- One test class per service. Use a private `CreateSut()` method to construct the subject.
- Mock at the interface boundary (`ICheckoutQuery`, `IStripeService`, etc.).

```csharp
private AppCheckoutService CreateSut() => new(
    _logger.Object, _options.Object, _checkoutCommand.Object, _checkoutQuery.Object, ...);
```

### Integration tests (`LymmHolidayLets.IntegrationTests`)
- Use `WebApplicationFactory<Program>` via `ApiFactory`.
- `ApiFactory` replaces all services with mocks — no real DB or external calls.
- Tests verify routing, model binding, middleware, and `ApiResponse<T>` shape.
- Use `IClassFixture<ApiFactory>` and configure mocks per test via `factory.<Service>.Setup(...)`.

## Database

- SQL Server. Schema is managed via DACPAC — **not** EF migrations.
- Schema changes must be reflected in `db/` scripts and the `.sqlproj`.
- Data changes go through Dapper stored procedures.
