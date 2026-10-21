# Clarity guild website

Recruitment site for the WoW guild Clarity. Applicants sign in with Discord and submit an application; the API stores it and the CCK bot posts it to a Discord channel with a thread and an officer ping.
Planned: Discord interactions (Approve → trial role + welcome, Request info → DM), officer control panel.

## Stack
- **API:** ASP.NET Core Web API (.NET 10), EF Core, Azure SQL (Entra-only auth: managed identity in Azure, `DefaultAzureCredential` locally)
- **Web:** React 19 + TypeScript + Vite + Tailwind v4, react-hook-form + Zod, Headless UI
- **Discord:** Discord.Net.Rest (bot uses REST only, no gateway); AspNet.Security.OAuth.Discord for sign-in
- **Tests:** xUnit in `ClarityGuildWebsite.Api.Tests`
- **Hosting:** Azure App Service (Linux) for the API at `api.clarityguild.co.uk`; Azure Static Web Apps for the web at `www.clarityguild.co.uk`
- **CI/CD:** GitHub Actions. `Dev.yaml` (workflow name `CI`) builds and tests on push/PR to `main`. The frontend deploys via `workflow_run` after CI succeeds. The API is deployed manually (no CD yet).

## Running locally
- API: `dotnet run --launch-profile https` in `ClarityGuildWebsite.Api` → **https://localhost:7259**. Test API endpoints on this port, not 5173.
- Web: `npm run dev` in `ClarityGuildWebsite.Web` → http://localhost:5173 (`VITE_API_BASE_URL` comes from `.env.development`)
- Tests: `dotnet test`
- Migrations (from the API project): `dotnet ef migrations add <Name>`, `dotnet ef database update`
- Secrets live in user-secrets locally and App Service settings in Azure. Never in appsettings or the repo. Includes `Discord:BotToken` and `Discord:ClientSecret`.

## Architecture
- **Controllers:** `GuildApplicationController` (POST create is `[Authorize]`; GET by `PublicId` is public) and `AuthController` (`api/auth`: login, me, logout).
- **IDs:** entities have an int `Id` plus a `PublicId` GUID. External URLs only ever use `PublicId`.
- **Delivery tracking** on `GuildApplication`: `PostStatus` (Pending/Delivered/Failed/Abandoned), `Retry`, `LastAttemptedUtc`, `LastPostError`, `DiscordMessageId`.
- **Notifiers:** `IApplicationNotifier` is implemented by `DiscordBotNotifier` or `DiscordWebhookNotifier`, selected by `Discord:NotifierMode` (`Bot` / `Webhook`). Register only the services for the selected mode.
- **Retry worker:** `DiscordRetryWorker` (BackgroundService + PeriodicTimer) retries Failed rows and Pending rows older than 10 minutes. It creates a new DI scope per tick. After `MaxRetries` (5) a row becomes Abandoned and an error is logged.
- **Mapping:** extension methods in `Mappings/` (no AutoMapper).
- **Auth:** cookie auth (HttpOnly, Secure, SameSite=Lax) plus Discord OAuth. The applicant's `DiscordId` and `DiscordName` come **only from the auth cookie's claims**, never from the request body. Because the cookie is Lax, the frontend and API must be same-site (subdomains of `clarityguild.co.uk`).
- **CSRF:** `OriginCheckMiddleware` rejects state-changing requests (anything except GET/HEAD/OPTIONS) whose `Origin` isn't in `Cors:AllowedOrigins`. The future Discord interactions endpoint must be excluded; it's protected by Discord's signature check instead.

## Conventions
- DTOs for all API input and output; never expose EF entities. Input DTOs contain only fields the applicant types.
- Validate on the API, not just the frontend (data annotations on the API, Zod on the web).
- Keep controllers thin; logic goes in services and mappers.
- Fail fast on missing configuration: throw at startup with a message naming the key.
- Logging uses message templates (`"Blocked {Path}", path`), never `$""` interpolation.
- Any new state-changing endpoint: decide auth, Origin check and rate limiting deliberately.
- Schema changes: always a new migration (never edit one that has run anywhere). Backfill when adding non-null columns. Apply migrations to Azure before deploying code that needs them.
- Tailwind: mobile-first; `gap-*` instead of margins between siblings; `min-w-0` on flex children holding inputs or long text.

## Gotchas
- Config arrays: read with `GetSection("X").Get<string[]>()`. `GetValue<string>` returns null. Arrays merge by index across config sources.
- App Service setting names use `__` for `:` (e.g. `Cors__AllowedOrigins__0`).
- The SWA deploy workflow file must keep its Azure-generated name (`azure-static-web-apps-ashy-coast-096246c0f.yml`), and `workflow_run` follows the workflow **name** `CI`. Renaming either one breaks deploys.
- Missing GitHub Actions variables and step outputs resolve to empty strings silently.
- `[ApiController]` returns 400 automatically when a DTO fails validation; the action never runs.

## About me and how to help
I'm a C#/.NET developer learning React, TypeScript, CSS layout, testing, CI/CD and Azure through this project.
- **Don't edit my files unless I ask.** Guide me with explanations, hints and small examples, and review what I write.
- **No guessing.** If you're not sure, say so and we'll dig in.
- Partial examples with `___` blanks work best for me unless otherwise stated.
- Explain the trade-offs of every library, pattern and schema choice, including the case against it.
- Point out security flaws, performance problems and bad habits, and teach the professional fix.
- Expand acronyms the first time you use them. Prefer simple, standard patterns over clever ones.
