ShelfWise

Simple Library Management tool — ASP.NET Core Web API + React SPA.

Overview
- Purpose: implement a small CRUD REST API and SPA for managing books.
- Model: `Book` with properties: Title, Author, Category, Genre, Available (int), OnHold (int), TotalCopies (int).

Chosen stack
- Backend: ASP.NET Core (C#) Web API using Entity Framework Core.
- Database: PostgreSQL (development via Docker recommended).
- Frontend: React (SPA) — can be reused for mobile with React Native later.

Repository goals and first steps
1. Initialize repository metadata (`README.md`, `LICENSE`, `.gitignore`).
2. Create solution and two projects: `ShelfWise.Api` (Web API) and `ShelfWise.Web` (React SPA).
3. Add `Book` model and a minimal `BooksController` with a GET endpoint.
4. Scaffold React placeholder that calls `/api/books`.

Notes
- Authentication: intentionally omitted for the initial iteration.
- Caching and advanced features: implement after endpoints are stable.

Next actions (I'll implement)
- Create .NET solution and projects.
- Add model and placeholder endpoints.
- Scaffold React app and simple fetch UI.

Contact
- If you want any change to the stack choices, tell me before I scaffold the projects.
