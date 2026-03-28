# CLAUDE.md — LymmHolidayLets

## Project
Holiday lettings management app. .NET 10 backend + Next.js 15 frontend.
Solution file: `lymmholidaylets.slnx`

## Architecture
- Clean/Hexagonal architecture with CQRS
- Layers: `Api` → `Application` → `Domain` ← `Infrastructure`
- Commands/queries go in `LymmHolidayLets.Application`
- Domain entities and repository interfaces in `LymmHolidayLets.Domain`
- EF Core + Dapper implementations in `LymmHolidayLets.Infrastructure`

## Coding Conventions (.NET)
- Follow existing patterns in the layer you're working in
- Use `async/await` throughout — no blocking calls
- FluentValidation for all input validation (not data annotations)
- Repository pattern — never access DbContext outside Infrastructure
- Domain exceptions in `LymmHolidayLets.Domain/Exception/`
- Nullable reference types enabled — handle nulls explicitly

## Coding Conventions (TypeScript/Next.js)
- Next.js App Router (not Pages Router)
- TypeScript strict mode
- Tailwind CSS for styling — no inline styles

## Testing
- xunit + Moq + FluentAssertions
- Unit tests: `LymmHolidayLets.UnitTests`
- Integration tests: `LymmHolidayLets.IntegrationTests`
- Run all tests: `dotnet test lymmholidaylets.slnx`
- Mock at repository/service boundaries, not deep in internals

## Local Dev
- Start everything: `docker-compose up`
- DB: SQL Server on localhost, database `LymmHolidayLets`
- API runs in Docker; UI runs in Docker or `npm run dev` in `src/lymmholidaylets.ui`

## Do Not
- Do not put business logic in controllers — use application services
- Do not access the DB directly from `Api` or `Application` layers
- Do not commit secrets or connection strings with real credentials
- Do not skip tests when adding new features or fixing bugs
