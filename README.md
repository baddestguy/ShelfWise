ShelfWise

A simple Library Management tool — ASP.NET Core Web API + React SPA.

Features
- Add, edit, and delete books.
- Mark books as checked in or out
- Search books
- Authenticate with SSO as an Admin or Librarian


Chosen stack
- Backend: ASP.NET (C#) Web API using Entity Framework Core.
- Database: PostgreSQL.
- Frontend: React (SPA).

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
- Use `SEED_DB=true` when running locally to populate sample books (the app seeds only in `Development` or when `SEED_DB=true`).
- To run the API on a different port, set `ASPNETCORE_URLS` when starting the container or `dotnet run`.
- The docker-compose file exposes Postgres on `localhost:5432` for convenience;


Local developer checklist:
- Start full stack:

```bash
docker compose up --build
```

- Run API only:

```bash
dotnet run --project src/ShelfWise.Api
```

- Run frontend only:

```bash
cd src/ShelfWise.Web
npm ci
npm run dev
```

- Run unit tests:

```bash
dotnet test tests/ShelfWise.Tests
```

Tests
- Run the unit tests:
```bash
dotnet test tests/ShelfWise.Tests
```

Troubleshooting
- If `docker compose up` fails during the web build, run `npm ci` in `src/ShelfWise.Web` to generate `package-lock.json`, then rebuild.
