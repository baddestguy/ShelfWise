ShelfWise

A simple Library Management tool — ASP.NET Core Web API + React SPA.

Overview
- Purpose: implement a small CRUD REST API and SPA for managing books.
- Model: `Book` with properties: Title, Author, Category, Genre, Available (int), OnHold (int), TotalCopies (int).

Chosen stack
- Backend: ASP.NET Core (C#) Web API using Entity Framework Core.
- Database: PostgreSQL (development via Docker recommended).
- Frontend: React (SPA) — can be reused for mobile with React Native later.

Developer Quick Start
Prerequisites
- Docker & Docker Compose
- .NET 8 SDK (optional, for running projects locally)
- Node 18+ and npm (optional, for running frontend locally)

Run the full stack (recommended — Docker)
1. Build and start services:
```bash
docker compose up --build
```
2. Open the frontend: http://localhost:3000
3. API endpoint (books): http://localhost:5000/api/books

Notes:
- The Compose setup runs Postgres, the ASP.NET Core API, and the Vite React dev server.
- If you modify `src/ShelfWise.Web`, run `npm ci` in that folder and commit `package-lock.json` for reproducible Docker builds.

Production & migrations (recommended workflow)
--
For development we auto-apply migrations and seed demo data when running in `Development` or when `SEED_DB=true`.
For production you should run migrations as a controlled deployment step rather than auto-applying from the running application.

Recommended production steps:
1. Build the app and run DB migrations in your deployment pipeline (example script provided at `scripts/migrate.sh`):
```bash
./scripts/migrate.sh
```
2. Use a CI/CD job or deployment playbook to run migrations with an account that has schema permissions; log and fail the deployment if migrations fail.
3. Use a restricted runtime DB user for the app (no schema-change permissions) and a separate migration user for the pipeline.
4. Disable seeding in production (the app gates seeding to `Development` or `SEED_DB=true`).

CI example
--
The repository includes a GitHub Actions job (`apply-migrations`) demonstrating how to run `dotnet-ef database update` in CI. For production wiring:
- Store DB connection strings and credentials as encrypted secrets. 
- Run `dotnet ef database update` in a dedicated CI job using the migration user and fail the deployment if the command fails.

If you want, I can add a sample deployment job that runs migrations against a GitHub Actions Postgres service for integration testing.

Run components locally (without Docker)
- API only:
```bash
dotnet build src/ShelfWise.Api
dotnet run --project src/ShelfWise.Api
```
- Frontend only:
```bash
cd src/ShelfWise.Web
npm ci
npm run dev
```

Tests
- Run the placeholder unit tests:
```bash
dotnet test tests/ShelfWise.Tests
```

Troubleshooting
- If the web UI doesn't load from Docker, ensure containers are up:
```bash
docker compose ps
docker compose logs --tail=100 web
```
- If `docker compose up` fails during the web build, run `npm ci` in `src/ShelfWise.Web` to generate `package-lock.json`, then rebuild.
