# Clarity guild website

Recruitment site for the WoW guild Clarity. Applicants sign in with Discord and submit an application. The API stores it, and the CCK bot posts it to a Discord channel with a thread, action buttons and a Recruitment Team ping. From the thread, the Recruitment Team can message the applicant (replies are relayed back into the thread), approve them (Trial role + welcome) or decline them (reason sent by DM).
Planned: officer control panel.

## Stack
- **API:** ASP.NET Core Web API (.NET 10), EF Core, Azure SQL (Entra-only auth: managed identity in Azure, `DefaultAzureCredential` locally)
- **Web:** React 19 + TypeScript + Vite + Tailwind v4, react-hook-form + Zod, Headless UI
- **Discord:** Discord.Net. A gateway connection (`DiscordSocketClient`, intents `Guilds | DirectMessages`) receives button clicks, modal submits and DMs. A separate `DiscordRestClient` posts applications, so posting doesn't depend on the gateway. AspNet.Security.OAuth.Discord for sign-in (scope `identify` only).
- **Tests:** xUnit in `ClarityGuildWebsite.Api.Tests`
- **Hosting:** Azure App Service (Linux) for the API; Azure Static Web Apps for the web
- **CI/CD:** GitHub Actions. `Dev.yaml` (workflow name `CI`) builds and tests on push/PR to `main`; the production frontend deploys via `workflow_run` after CI succeeds. `deploy-web-dev.yml` deploys the dev frontend manually. The API is deployed manually with the Visual Studio publish profile (no CD yet).

## Environments
| | Local | Dev | Production |
|---|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Development | Staging | Production |
| Web | http://localhost:5173 | dev.clarityguild.co.uk | www.clarityguild.co.uk |
| API | https://localhost:7259 | dev-api.clarityguild.co.uk | api.clarityguild.co.uk |
| Database | LocalDB | clarityguild-dev | clarityguild |
| Discord | test bot + test server | test bot + test server | CCK + Clarity server |

Dev runs as `Staging`, so it reads production's `appsettings.json` plus App Service overrides. Every Discord ID needs a dev value there.

## Running locally
- API: `dotnet run --launch-profile https` in `ClarityGuildWebsite.Api` → **https://localhost:7259**. Test API endpoints on this port, not 5173.
- Web: `npm run dev` in `ClarityGuildWebsite.Web` → http://localhost:5173 (`VITE_API_BASE_URL` and `VITE_DISCORD_INVITE_URL` come from `.env.development`)
- Tests: `dotnet test`
- Migrations (from the API project): `dotnet ef migrations add <Name>`, `dotnet ef database update`
- Secrets live in user-secrets locally and App Service settings in Azure. Never in appsettings or the repo: `Discord:BotToken`, `Discord:ClientSecret`, connection strings.

## Configuration (`Discord` section)
- Non-secret IDs live in `appsettings.json` (production) and `appsettings.Development.json`: `NotifierMode`, `GuildId`, `ClientId`, `ApplicationsChannelId`, `RaidersChatId`, `TrialRoleId`, `RecruitmentTeamRoleIds[]`, `DefaultWelcomeChannelIds[]`.
- The router is scoped, so its config checks only run on the first click, not at startup (Options + `ValidateOnStart` is on the to-do list).

## Architecture
- **Controllers:** `GuildApplicationController` (POST create is `[Authorize]`; GET by `PublicId` is public) and `AuthController` (`api/auth`: login, me, logout).
- **IDs:** entities have an int `Id` plus a `PublicId` GUID. External URLs and Discord custom IDs only ever use `PublicId`.
- **Delivery tracking** on `GuildApplication`: `PostStatus` (Pending/Delivered/Failed/Abandoned), `Retry`, `LastAttemptedUtc`, `LastPostError`, `DiscordMessageId`, `DiscordThreadId` (unique).
- **Notifiers:** `IApplicationNotifier` is implemented by `DiscordBotNotifier` or `DiscordWebhookNotifier`, selected by `Discord:NotifierMode` (`Bot` / `Webhook`). Register only the services for the selected mode. The bot notifier posts the embed, then the thread, the buttons and the ping as independent steps.
- **Workers:** `DiscordRetryWorker` retries Failed rows and Pending rows older than 10 minutes (Abandoned after 5). `PersonalDataDeleteWorker` deletes applications older than 3 months, daily. Both run once at startup and create a DI scope per run.
- **Discord bot:**
  - `DiscordGatewayService` (hosted service) owns the socket client: subscribe events → login → start. Each event runs via `RunInBackground` (off the gateway task, new scope, try/catch inside).
  - `DiscordInteractionRouter` (scoped) handles buttons and modals. Access is via `CanManageApplications` (any role in `RecruitmentTeamRoleIds`), checked on the click and again on submit. A button finds its application by thread ID. Modal custom IDs are `{Type}:{PublicId}`.
  - `ApplicantMessagingService` relays applicant DMs to the thread of the application with the latest activity (`LastRecruitmentTeamMessagedUtc ?? TimestampUtc`).
  - Constants: `DiscordCustomIds` (button IDs, never change their values, old messages use them) and `DiscordModals` (static builders + field IDs).
- **Mapping:** extension methods in `Mappings/` (no AutoMapper).
- **Auth:** cookie auth (HttpOnly, Secure, SameSite=Lax) plus Discord OAuth. The applicant's `DiscordId` and `DiscordName` come **only from the auth cookie's claims**, never from the request body. Because the cookie is Lax, the frontend and API must be same-site (subdomains of `clarityguild.co.uk`).
- **CSRF:** `OriginCheckMiddleware` rejects state-changing requests (anything except GET/HEAD/OPTIONS) whose `Origin` isn't in `Cors:AllowedOrigins`. No exclusions (the HTTP interactions endpoint was removed when the bot moved to the gateway).

## Conventions
- DTOs for all API input and output; never expose EF entities. Input DTOs contain only fields the applicant types.
- Validate on the API, not just the frontend (data annotations on the API, Zod on the web).
- Keep controllers thin; logic goes in services and mappers.
- Fail fast on missing configuration: throw with a message naming the key.
- Logging uses message templates (`"Blocked {Path}", path`), never `$""` interpolation. Never log personal data or message/DM content; log `PublicId`s.
- Any new state-changing endpoint: decide auth, Origin check and rate limiting deliberately.
- Schema changes: always a new migration (never edit one that has run anywhere). Backfill when adding non-null columns. Apply migrations to Azure before deploying code that needs them.
- Side effects: do the irreversible step last (save to the database, then DM/post).
- Discord: respond to every interaction within 3 seconds. Modals must be the first response (can't defer); modal submits defer first, then follow up on every exit. User-written text is posted with `AllowedMentions.None`.
- Tailwind: mobile-first; `gap-*` instead of margins between siblings; `min-w-0` on flex children holding inputs or long text.

## Deploying the API
1. `dotnet ef migrations script --idempotent -o ../deploy.sql` (outside the repo), review it, run it on the target database.
2. Publish with the Visual Studio profile (Linux, framework-dependent, Release).
3. Watch the log stream: the app should start from `/home/site/wwwroot` and Discord.Net should reach `Ready`.
4. Smoke test: submit an application, use a button, decline it.

## Gotchas
- Config arrays: read with `GetSection("X").Get<T[]>()`. `GetValue<...>` returns null/0 for arrays. Arrays merge by index across config sources.
- App Service setting names use `__` for `:` (e.g. `Cors__AllowedOrigins__0`). Changing settings restarts the app.
- App Service needs `WEBSITE_RUN_FROM_PACKAGE=1` and `SCM_DO_BUILD_DURING_DEPLOYMENT=false`. If the log shows `hostingstart.dll`, `wwwroot` is empty: check those settings and redeploy.
- The gateway only works with Always On and a single instance; a deploy briefly disconnects the bot.
- A Discord app receives interactions over the gateway **or** an Interactions Endpoint URL, never both. Keep the portal URL empty.
- `GetRequiredService<T>` must ask for exactly the registered type (interface vs class). It fails only at runtime, and inside `RunInBackground` only as a logged error.
- Event handlers run in parallel, so "already accepted/declined" checks can race (optimistic concurrency is on the to-do list).
- Interpolating a collection (`$"{list}"`, `.ToString()`) prints its type name. Use `string.Join` + `Select`.
- Discord limits: modal title and labels 45 chars, message 2,000, embed description 4,096. A bot can only DM users who share a server with it and allow DMs (error 50007).
- The SWA deploy workflow file must keep its Azure-generated name (`azure-static-web-apps-ashy-coast-096246c0f.yml`), and `workflow_run` follows the workflow **name** `CI`. Renaming either one breaks deploys.
- Missing GitHub Actions variables and step outputs resolve to empty strings silently (e.g. `VITE_DISCORD_INVITE_URL` → empty link).
- `[ApiController]` returns 400 automatically when a DTO fails validation; the action never runs.

## About me and how to help
I'm a C#/.NET developer learning React, TypeScript, CSS layout, testing, CI/CD and Azure through this project.
- **Don't edit my files unless I ask.** Guide me with explanations, hints and small examples, and review what I write.
- **No guessing.** If you're not sure, say so and we'll dig in.
- Partial examples with `___` blanks work best for me unless otherwise stated.
- Explain the trade-offs of every library, pattern and schema choice, including the case against it.
- Point out security flaws, performance problems and bad habits, and teach the professional fix.
- Expand acronyms the first time you use them. Prefer simple, standard patterns over clever ones.
