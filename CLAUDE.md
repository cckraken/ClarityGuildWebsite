# Guild website

Guild applications site. Later: officer-only control panel.

## Stack
- Backend: ASP.NET Core Web API (C#), Entity Framework Core
- Frontend: React + Tailwind
- [database: SQL Server / SQLite / Postgres?]

## Running
- API: `dotnet run` in /ClarityGuildWebsite.Api
- Frontend: `npm run dev` in /ClarityGuildWebsite.Web
- Tests: `dotnet test`

## Conventions
- Use DTOs for API input/output; never expose EF entities directly
- Validate all input on the API side, not just the frontend
- Keep controllers thin; logic goes in services

## About me
I'm learning ASP.NET APIs and React. Explain non-obvious decisions,
and prefer simple, standard patterns over clever ones.
Don't edit my files unless I ask. Guide me with explanations, hints and small examples, and review what I write.