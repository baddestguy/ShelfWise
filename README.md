# ShelfWise

ShelfWise is a library management app built with an ASP.NET Core API, PostgreSQL, and a TypeScript React dashboard.

## Live Demo

- Web app: https://shelfwise-web-production.up.railway.app
- API: https://shelfwise-api-production.up.railway.app/api/books
- Swagger: https://shelfwise-api-production.up.railway.app/swagger

## Features

- Book management: add, edit, delete, list, and search books.
- Inventory tracking: total copies, checked-out copies, and available copies.
- Circulation: check books out to users and check them back in.
- User management: list users as a Librarian/Admin and create users as an Admin.
- JWT authentication with seeded Patron, Librarian, and Admin demo accounts.
- TypeScript React dashboard for search, inventory, circulation, user selection, JWT login, and role-based actions.
- In-memory cache for book inventory and search responses, invalidated after mutations.
- AI Librarian semantic search using OpenAI embeddings when `OPENAI_API_KEY` is configured, with a local fallback when it is not.
- Docker Compose setup for the API, web app, and PostgreSQL database.

## Tech Stack

- Backend: ASP.NET Core 8 Web API
- Data: PostgreSQL, Entity Framework Core
- Frontend: TypeScript, React, Vite
- Runtime/dev environment: Docker Compose

## Quick Start

Prerequisites:

- Docker and Docker Compose

Run the full stack:

```bash
docker compose up --build
```

Open:

- Web app: http://localhost:3000
- API books endpoint: http://localhost:5000/api/books
- API users endpoint: http://localhost:5000/api/users (requires `Librarian` or `Admin`)
- Swagger UI: http://localhost:5000/swagger

The API applies EF Core migrations on startup, ensures the demo schema exists, and seeds missing sample books plus demo login users. This runs in local Docker and in hosted deployments so a fresh database can start ready for testing.

Optional AI configuration:

Create a `.env` file in the repository root:

```env
OPENAI_API_KEY=your_api_key
OPENAI_EMBEDDING_MODEL=text-embedding-3-small
```

You can copy `.env.example` as a starting point. Do not commit `.env`.

Then run:

```bash
docker compose up --build
```

If `OPENAI_API_KEY` is not set, the AI Librarian still works with local keyword scoring and reports fallback mode in the response/UI.

Optional JWT configuration:

```env
JWT_SIGNING_KEY=replace-with-a-long-random-secret-at-least-32-characters
```

If `JWT_SIGNING_KEY` is not set, the API uses a development-only signing key.

To reset local data:

```bash
docker compose down -v
docker compose up --build
```

## Local Development

Run the API directly:

```bash
dotnet run --project src/ShelfWise.Api
```

Run the frontend directly:

```bash
cd src/ShelfWise.Web
npm ci
npm run dev
```

For a hosted frontend that proxies API calls to a public API service, configure:

```env
VITE_API_TARGET=https://your-api-domain.example.com
VITE_ALLOWED_HOSTS=your-web-domain.example.com
```

For static/client-side API calls without the Vite proxy, configure:

```env
VITE_API_BASE_URL=https://your-api-domain.example.com
```

Run tests:

```bash
dotnet test ShelfWise.sln
```

Run frontend smoke tests against a running local app:

```bash
cd src/ShelfWise.Web
npx playwright install chromium
npm run test:e2e
```

The Playwright suite is intentionally local-only for now because it expects the API, web app, and database to be running together.

## Auth And Roles

ShelfWise uses username/password login with JWT bearer tokens. Passwords are salted and hashed with ASP.NET Core's `PasswordHasher<TUser>`, and the JWT contains the user's role claim for API authorization.

Seeded demo accounts:

- `patron@shelfwise.dev` / `Password123!`
- `librarian@shelfwise.dev` / `Password123!`
- `admin@shelfwise.dev` / `Password123!`

Supported roles:

- `Patron`
- `Librarian`
- `Admin`

Permissions:

- Patron reads:
  - `GET /api/books`
  - `GET /api/books/{id}`
- Librarian or Admin:
  - `GET /api/users`
  - `GET /api/users/{id}`
  - `POST /api/books`
  - `PATCH /api/books/{id}`
  - `POST /api/books/{id}/checkout`
  - `POST /api/books/{id}/checkin`
  - `POST /api/books/{id}/hold`
- Admin only:
  - `DELETE /api/books/{id}`
  - `POST /api/users`

Example:

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"admin@shelfwise.dev\",\"password\":\"Password123!\"}"
```

Protected endpoints require:

```http
Authorization: Bearer <token>
```

For production, configure `JWT_SIGNING_KEY` or `Jwt:SigningKey` with a strong secret. A real SSO provider such as Microsoft Entra ID, Auth0, or Okta could replace the login endpoint later while keeping the same authorization policies.

## API Examples

Swagger UI is available locally and in the deployed API:

- Local: http://localhost:5000/swagger
- Production: https://shelfwise-api-production.up.railway.app/swagger

To test protected endpoints in Swagger, call `POST /api/auth/login`, copy the `token` value from the response, click `Authorize`, and paste the token.

Search books:

```bash
curl "http://localhost:5000/api/books?search=clean"
```

Create a book:

```bash
curl -X POST http://localhost:5000/api/books \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <librarian-or-admin-token>" \
  -d "{\"title\":\"Refactoring\",\"author\":\"Martin Fowler\",\"category\":\"NonFiction\",\"genre\":\"Technical\",\"totalCopies\":2}"
```

Create a user:

```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <admin-token>" \
  -d "{\"firstName\":\"Ada\",\"lastName\":\"Lovelace\"}"
```

Check out a book:

```bash
curl -X POST http://localhost:5000/api/books/1/checkout \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <librarian-or-admin-token>" \
  -d "{\"userId\":1,\"dueDays\":14}"
```

Check in a book:

```bash
curl -X POST http://localhost:5000/api/books/1/checkin \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <librarian-or-admin-token>" \
  -d "{\"userId\":1}"
```

AI Librarian semantic search:

```bash
curl -X POST http://localhost:5000/api/ai/book-search \
  -H "Content-Type: application/json" \
  -d "{\"query\":\"I want practical books about writing better software\"}"
```

## Architecture

The solution is split into small projects:

- `ShelfWise.Api`: HTTP controllers, auth setup, dependency injection, startup.
- `ShelfWise.Api/Services`: AI Librarian semantic search using OpenAI embeddings with fallback keyword scoring.
- `ShelfWise.Domain`: domain models.
- `ShelfWise.Repository`: EF Core DbContext, schema initialization, repositories.
- `ShelfWise.Services`: business logic, cache behavior, circulation rules.
- `ShelfWise.Web`: TypeScript React dashboard.

## Current Tradeoffs

- Authentication uses local username/password login with JWT bearer tokens rather than third-party SSO. The authorization policies are intentionally structured so an OpenID Connect provider could replace the login endpoint later.
- Startup applies migrations, ensures schema compatibility, and seeds missing demo data for deployment reliability.
- Book cover images are resolved client-side from Open Library and cached in the browser, so cover lookup depends on the public Open Library service.
- Email notifications are a planned bonus item and are not required to run the current product.
- The app is deployed on Railway as separate web, API, and PostgreSQL services.

## Planned Enhancements

- Real SSO through OpenID Connect/JWT.
- Persisted/vector-indexed embeddings for larger catalogs.
- Overdue notification worker for past-due loans.
