# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FC.Codeflix.Catalog is a Clean Architecture ASP.NET Core 8.0 video catalog system for managing categories, genres, cast members, and videos. It uses Domain-Driven Design patterns with CQRS via MediatR.

## Build and Test Commands

```bash
# Build entire solution
dotnet build

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/FC.CodeFlix.Catalog.UnitTests
dotnet test tests/FC.Codeflix.Catalog.IntegrationTests
dotnet test tests/FC.CodeFlix.Catalog.EndToEndTests

# Run tests with filter (by name pattern)
dotnet test --filter "FullyQualifiedName~Category"
dotnet test --filter "FullyQualifiedName~CreateCastMember"

# Run API locally (requires MySQL via docker-compose)
docker-compose up -d
dotnet run --project src/FC.CodeFlix.Catalog.API
# Swagger UI: https://localhost:5001/swagger/index.html

# EF Migrations
cd src/FC.CodeFlix.Catalog.API
dotnet ef migrations add <MigrationName> --project ../FC.Codeflix.Catalog.Infra.Data.EF
dotnet ef database update
```

## Architecture

```
src/
├── FC.CodeFlix.Catalog.Domain/           # Core domain (entities, value objects, repository interfaces)
├── FC.CodeFlix.Catalog.Application/      # Use cases organized by entity (MediatR handlers)
├── FC.Codeflix.Catalog.Infra.Data.EF/    # EF Core repositories, DbContext, configurations
└── FC.CodeFlix.Catalog.API/              # ASP.NET Core controllers, API models

tests/
├── FC.CodeFlix.Catalog.UnitTests/        # Isolated business logic tests (mocked dependencies)
├── FC.Codeflix.Catalog.IntegrationTests/ # Use cases with in-memory SQLite
└── FC.CodeFlix.Catalog.EndToEndTests/    # Full API tests via WebApplicationFactory
```

### Key Patterns

- **Aggregate Roots**: Category, Genre, CastMember, Video (in `Domain/Entity/`)
- **Value Objects**: Image, Media (in `Domain/ValueObject/`)
- **Repository Pattern**: Interfaces in Domain, implementations in Infra.Data.EF
- **Unit of Work**: `IUnitOfWork` for transaction management
- **Validation**: Custom `ValidationHandler` with notification pattern in Domain

### Use Case Structure

Each use case follows: Input DTO → MediatR Handler → Output DTO

Example flow for CreateCategory:
1. `CreateCategoryInput` (Application/UseCases/Category/CreateCategory/)
2. `CreateCategory` handler (implements `IRequestHandler<CreateCategoryInput, CategoryModelOutput>`)
3. `CategoryModelOutput` returned

## Database

- **Provider**: MySQL via Pomelo.EntityFrameworkCore.MySql
- **Docker**: `docker-compose up -d` starts MySQL on port 3306
- **Connection**: Configured in `appsettings.json` under `ConnectionStrings:CatalogDb`
- **Test Database**: In-memory SQLite (integration tests use `BaseFixture.CreateDbContext()`)

## API Conventions

- RESTful endpoints with snake_case parameter names (via `JsonSnakeCasePolicy`)
- Response wrapper: `APIResponse<T>` and `APIResponseList<T>` for paginated results
- Query parameters: `page`, `per_page`, `search`, `sort`, `dir`

## Test Fixtures

Tests use fixtures for data generation:
- `BaseFixture.CreateDbContext()` - Creates in-memory test database
- Bogus library generates realistic fake data
- Tests run sequentially (`xunit.runner.json`: `parallelizeTestCollections: false`)
