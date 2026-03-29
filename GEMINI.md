# LymmHolidayLets Project Mandates

## Tech Stack
- **Backend:** .NET 10.0 (ASP.NET Core Web API, GraphQL)
- **Frontend:** Next.js (TypeScript, Vanilla CSS preferred)
- **Database:** SQL Server (DACPAC, migrations in `db/`)
- **Infrastructure:** AWS CDK (TypeScript)
- **Integrations:** Stripe (Payments), SendGrid (Email), Logz.io (Logging)

## Engineering Standards
- **Frameworks:** Adhere to .NET 10 patterns. Use FluentValidation for all input models.
- **Testing:** Maintain coverage in `src/LymmHolidayLets.UnitTests` (unit) and `src/LymmHolidayLets.IntegrationTests` (HTTP/integration). Add tests for new features or bug fixes.
- **API:** Follow RESTful conventions for controllers and use GraphQL for flexible data fetching where appropriate.
- **UI:** Use Next.js 15+ patterns (App Router if applicable). Prefer Vanilla CSS for styling unless Tailwind is explicitly requested.
- **Database:** Changes should be reflected in the `.sqlproj` and `db/` scripts.

## Workflows
- **Build:** `dotnet build lymmholidaylets.slnx`
- **Test:** `dotnet test lymmholidaylets.slnx`
- **UI Setup:** `cd lymmholidaylets.ui && npm install`
- **Infrastructure:** Managed via `infra/` using AWS CDK.

## Conventions
- Use Serilog for structured logging.
- Ensure all API responses follow the `ApiResponse` model in `src/LymmHolidayLets.Api/Models/ApiResponse.cs`.
