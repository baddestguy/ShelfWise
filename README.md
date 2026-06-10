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
