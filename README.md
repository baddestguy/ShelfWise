# ShelfWise

ShelfWise is a library management app built with an ASP.NET Core API, PostgreSQL, and a React dashboard.

## Features

- Book management: add, edit, delete, list, and search books.
- Inventory tracking: total copies, checked-out copies, and available copies.
- Circulation: check books out to users and check them back in.
- User management: list users and create users as an Admin.
- Role-based demo authentication using `X-User-Role`.
- React dashboard for search, inventory, circulation, user selection, and role switching.
- In-memory cache for book inventory and search responses, invalidated after mutations.
- Docker Compose setup for the API, web app, and PostgreSQL database.

## Tech Stack

- Backend: ASP.NET Core 8 Web API
- Data: PostgreSQL, Entity Framework Core
- Frontend: React + Vite
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
- API users endpoint: http://localhost:5000/api/users

The API applies database setup on startup and seeds sample books/users in Development or when `SEED_DB=true`.

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

Run tests:

```bash
dotnet test ShelfWise.sln
```

## Demo Auth And Roles

ShelfWise includes demo role-based authentication through the `X-User-Role` request header. This keeps the app easy to run locally while still demonstrating authorization boundaries.

Supported roles:

- `Patron`
- `Librarian`
- `Admin`

Permissions:

- Public reads:
  - `GET /api/books`
  - `GET /api/books/{id}`
  - `GET /api/users`
  - `GET /api/users/{id}`
- Librarian or Admin:
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
curl -H "X-User-Role: Admin" http://localhost:5000/api/users
```

In production, this demo auth scheme would be replaced with OpenID Connect/JWT validation from an SSO provider such as Microsoft Entra ID, Auth0, or Okta while keeping the same authorization policies.

## API Examples

Search books:

```bash
curl "http://localhost:5000/api/books?search=clean"
```

Create a book:

```bash
curl -X POST http://localhost:5000/api/books \
  -H "Content-Type: application/json" \
  -H "X-User-Role: Librarian" \
  -d "{\"title\":\"Refactoring\",\"author\":\"Martin Fowler\",\"category\":\"NonFiction\",\"genre\":\"Technical\",\"totalCopies\":2}"
```

Create a user:

```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -H "X-User-Role: Admin" \
  -d "{\"firstName\":\"Ada\",\"lastName\":\"Lovelace\"}"
```

Check out a book:

```bash
curl -X POST http://localhost:5000/api/books/1/checkout \
  -H "Content-Type: application/json" \
  -H "X-User-Role: Librarian" \
  -d "{\"userId\":1,\"dueDays\":14}"
```

Check in a book:

```bash
curl -X POST http://localhost:5000/api/books/1/checkin \
  -H "Content-Type: application/json" \
  -H "X-User-Role: Librarian" \
  -d "{\"userId\":1}"
```

## Architecture

The solution is split into small projects:

- `ShelfWise.Api`: HTTP controllers, auth setup, dependency injection, startup.
- `ShelfWise.Domain`: domain models.
- `ShelfWise.Repository`: EF Core DbContext, schema initialization, repositories.
- `ShelfWise.Services`: business logic, cache behavior, circulation rules.
- `ShelfWise.Web`: React dashboard.

## Current Tradeoffs

- Authentication uses a demo header scheme rather than real SSO. The authorization policies are intentionally structured so a real SSO/JWT provider can replace the demo handler later.
- The local database setup includes deterministic schema creation for demo reliability.
- Email notifications and AI features are planned bonus items and are not required to run the current product.

## Planned Enhancements

- Real SSO through OpenID Connect/JWT.
- Semantic AI search or recommendations using embeddings.
- Overdue notification worker for past-due loans.
- Deployment URL for live testing.
