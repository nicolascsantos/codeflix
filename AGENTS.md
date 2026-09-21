# Repository Guidelines

## Project Structure & Module Organization

`FC.CodeFlix.Catalog.sln` contains the .NET 8 video catalog backend. Under `src/`, Domain owns entities, validation, events, and repository contracts; Application organizes MediatR use cases by entity and operation; API contains controllers, response models, and dependency configuration. Infrastructure projects implement EF Core persistence, RabbitMQ messaging, and storage integrations. Keep business rules in Domain and infrastructure dependencies behind interfaces.

`tests/` contains separate UnitTests, IntegrationTests, and EndToEndTests projects, mirroring production features. Preserve existing `CodeFlix` versus `Codeflix` capitalization in project paths, particularly `FC.Codeflix.Catalog.Infra.Data.EF` and `FC.Codeflix.Catalog.IntegrationTests`.

## Build, Test, and Development Commands

Run from the repository root with the .NET 8 SDK installed:

- `dotnet build` — restore dependencies and build the solution.
- `docker compose up -d` — start local MySQL and RabbitMQ.
- `dotnet run --project src/FC.CodeFlix.Catalog.API` — launch the API.
- `dotnet test tests/FC.CodeFlix.Catalog.UnitTests` — run isolated tests.
- `dotnet test tests/FC.Codeflix.Catalog.IntegrationTests` — run integration tests.
- `docker compose -f tests/FC.CodeFlix.Catalog.EndToEndTests/docker-compose.yml up -d` — start dedicated end-to-end dependencies.
- `dotnet test` — run all suites after dependencies are ready.
- `dotnet test --filter "FullyQualifiedName~Category"` — focus on a feature.

## Coding Style & Naming Conventions

Use four-space C# indentation, PascalCase for types and public members, camelCase for parameters and locals, and `_camelCase` for private fields. Follow neighboring namespace and brace styles. Keep nullable reference types enabled. Use existing names such as `GetVideoInput`, `GetVideo`, and `VideoModelOutput` when adding use cases. No repository-wide formatter configuration is present.

## Testing Guidelines

Use xUnit, FluentAssertions, Moq, and Bogus. Follow `*Test`, `*TestFixture`, and `*TestDataGenerator` patterns. Integration fixtures use EF Core's in-memory provider; API tests use `WebApplicationFactory` and dedicated services. End-to-end setup deletes and recreates its database, so use only test connections. Cover success, validation, and failure behavior. Coverlet is available; no numeric coverage threshold is documented.

## Commit & Pull Request Guidelines

Recent commits mix Portuguese descriptions, English summaries, and `test:` prefixes. Prefer concise, focused messages; use `test:` for test-only changes. Pull requests should explain behavior changes, link relevant issues, report validation commands and results, and identify database or configuration changes.

## Configuration

Keep real credentials out of tracked settings. Use environment variables or the API project's user secrets for local overrides.
