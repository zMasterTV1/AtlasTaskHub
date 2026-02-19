# Atlas TaskHub (ASP.NET Core + SQL Server)

Atlas TaskHub is a small but production-minded **task management backend** built to showcase how I design and ship .NET services:
authentication, clean layering, database modeling, background-safe patterns, and “real world” DX (Docker, Swagger, CI).

## What it does
- **JWT authentication + refresh tokens** (token rotation, revoke-on-logout)
- **Role-based authorization** (User / Admin)
- **Projects & Tasks**
  - pagination, filtering, sorting
  - optimistic concurrency via `RowVersion`
- **SQL Server + EF Core**
  - code-first mapping
  - automatic migrations on startup (dev)
- **API ergonomics**
  - OpenAPI/Swagger with JWT support
  - problem-details error responses
  - request correlation id in logs
- **Ops-ready**
  - `/health` endpoint
  - Docker Compose (API + SQL Server)
  - GitHub Actions CI (build + tests)

## Quick start (Docker)
```bash
docker compose up --build
```

Then open Swagger:
- `http://localhost:8080/swagger`

Default dev settings:
- SQL Server: `localhost,1433` (user: `sa`, password: `Your_password123`)
- API: `http://localhost:8080`

## Local start (without Docker)
1. Start SQL Server (local or container).
2. Set connection string:
```bash
export ConnectionStrings__Default="Server=localhost,1433;Database=AtlasTaskHub;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
```
3. Run:
```bash
dotnet run --project src/Atlas.Api/Atlas.Api.csproj
```

## Key endpoints
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`

- `GET /api/projects`
- `POST /api/projects`
- `GET /api/projects/{projectId}/tasks`
- `POST /api/projects/{projectId}/tasks`

## Notes
- Passwords use PBKDF2 (Rfc2898DeriveBytes) with per-user salt.
- Refresh tokens are stored as **hashes** (never plain text).
- Concurrency token (`RowVersion`) prevents accidental overwrites.

## Roadmap ideas
- Rate limiting per user
- Audit trail for task changes
- Full multi-tenancy (orgs)
