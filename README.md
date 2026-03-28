# SimpleRestApi

A minimal ASP.NET Core REST API project for learning REST concepts step by step.

## What this project includes

- Minimal API endpoints
- Dependency injection with simple services
- Swagger/OpenAPI documentation
- Endpoint integration tests (xUnit + WebApplicationFactory)

## Tech stack

- .NET 9
- ASP.NET Core Minimal API
- Swagger (Swashbuckle)
- xUnit

## Project structure

- `SimpleRestApi/` : API project
- `SimpleRestApi.Tests/` : endpoint tests
- `SimpleRestApi.sln` : solution file

## Prerequisites

- .NET SDK 9.0+
- Git

## Run the API locally

From repository root:

```bash
dotnet run --project SimpleRestApi/SimpleRestApi.csproj
```

By default, the API starts on local HTTPS/HTTP ports shown in console output.

## Open API documentation

When running in Development:

- Swagger UI: `/swagger`
- OpenAPI JSON: `/openapi/v1.json`

Example local URL:

- https://localhost/swagger

## Main endpoints

- `GET /` : API status + links
- `GET /docs` : simple HTML docs page
- `GET /workouts` : returns workouts
- `POST /workouts` : creates a workout

## Example request (create workout)

```bash
curl -X POST "https://localhost/workouts" \
  -H "Content-Type: application/json" \
  -d "{\"id\":0,\"type\":\"Swim\",\"distance\":1.5,\"durationMinutes\":45,\"date\":\"2026-03-28T00:00:00\"}"
```

## Run tests

From repository root:

```bash
dotnet test SimpleRestApi.sln
```

## Learning path

1. Add validation rules to `POST /workouts`
2. Add `GET /workouts/{id}`, `PUT`, and `DELETE`
3. Add persistence with EF Core + SQLite
4. Add DTOs and mapping
5. Add authentication (JWT)
6. Add API versioning

## License

This project is for learning purposes.
