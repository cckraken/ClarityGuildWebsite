# Clarity guild website

Recruitment site for the WoW guild Clarity. Applicants sign in with Discord and submit an application. The API stores it, and the CCK bot posts it to a Discord channel with a thread, action buttons and a Recruitment Team ping. From the thread, the Recruitment Team can message the applicant (replies are relayed back into the thread), accept them (Trial role + welcome) or decline them (reason sent by DM).
CCK also runs a **recruitment feed**: Warcraft Logs recruitment webhook posts are enriched with each character's first-kill and best Mythic parses (WCL API v2) and reposted to the officers' channel.
Planned: officer control panel.

## Stack
- **API:** ASP.NET Core Web API (.NET 10), EF Core, Azure SQL (Entra-only auth: managed identity in Azure, `DefaultAzureCredential` locally)
- **Web:** React 19 + TypeScript + Vite + Tailwind v4, react-hook-form + Zod, Headless UI
- **Discord:** Discord.Net. A gateway connection (`DiscordSocketClient`, intents `Guilds | DirectMessages | GuildMessages | MessageContent`) receives button clicks, modal submits, slash commands, DMs and the recruitment feed's webhook posts. `MessageContent` is privileged: it must be enabled on each bot's portal page (Bot → Privileged Gateway Intents). A separate `DiscordRestClient` posts applications, so posting doesn't depend on the gateway. AspNet.Security.OAuth.Discord for sign-in (scope `identify` only).
- **Warcraft Logs:** GraphQL API v2 with OAuth client credentials (typed `HttpClient` + singleton token service).
- **Logging:** Application Insights SDK 3.x (OpenTelemetry-based) in Production; sends Information and above by default.
- **Tests:** xUnit in `ClarityGuildWebsite.Api.Tests`
- **Hosting:** Azure App Service (Linux) for the API; Azure Static Web Apps for the web
- **CI/CD:** GitHub Actions (see **CI/CD** below). CI builds and tests on push/PR to `main`; on push it also publishes the API package and the frontend build. The production frontend and the API (dev → approved prod) deploy from those artifacts via `workflow_run`. `deploy-web-dev.yml` deploys the dev frontend manually.

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

## Configuration
Options pattern: each feature has an options class, bound and validated at startup by its own `Add…` extension (`AddOptions<T>().Bind().Validate().ValidateOnStart()`). Inject `IOptions<T>`, never `T`. A missing or invalid value stops the app at startup, not on the first click.

| Section | Options class | Registered by | Keys |
|---|---|---|---|
| `Discord` | `DiscordBotOptions` | `AddDiscordBot` | `BotToken` (secret), `GuildId`, `ApplicationsChannelId`, `RaidersChatId`, `TrialRoleId`, `RecruitmentTeamRoleIds[]`, `DefaultWelcomeChannelIds[]` (also `NotifierMode`, `ClientId`, `ClientSecret` (secret), `WebhookUrl` (secret, webhook mode only), read in `Program.cs`) |
| `RecruitmentFeed` | `RecruitmentFeedOptions` | `AddRecruitmentFeed` | `IncomingChannelId`, `OutgoingChannelId`, `AllowedWebhookIds[]` |
| `WarcraftLogs` | `WarcraftLogsOptions` | `AddWarcraftLogs` | `ClientId` + `ClientSecret` (secrets), `Bosses[]` (`Id`, `Name`, optional `ShortName`) |

- Non-secret IDs live in `appsettings.json` (production) and `appsettings.Development.json`; dev (`Staging`) overrides them in App Service settings.
- `ContainerTests` builds the app as Production with fake config and resolves every gateway handler, so a missing registration or captive dependency fails CI, not production.
- `Bosses` drives both the GraphQL query (aliases `e{Id}`) and the table rows: update it each raid tier.

## Architecture
- **Controllers:** `GuildApplicationController` (POST create is `[Authorize]`; GET by `PublicId` is public) and `AuthController` (`api/auth`: login, me, logout).
- **IDs:** entities have an int `Id` plus a `PublicId` GUID. External URLs and Discord custom IDs only ever use `PublicId`.
- **Delivery tracking** on `GuildApplication`: `PostStatus` (Pending/Delivered/Failed/Abandoned), `Retry`, `LastAttemptedUtc`, `LastPostError`, `DiscordMessageId`, `DiscordThreadId` (unique).
- **Layout:** feature folders under `Features/`: `Applications` (posting service, `PersonalDataDeleteWorker`), `Notifications` (notifiers, `DiscordRetryWorker`), `DiscordBot` (`Interactions`, `Messaging`, `SlashCommands`), `RecruitmentFeed`, `Integrations/WarcraftLogs` (`Responses`, `GQLQueries`). The folder is `DiscordBot`, not `Discord`: a `Discord` namespace would clash with the Discord.Net library.
- **Notifiers:** `IApplicationNotifier` is implemented by `DiscordBotNotifier` or `DiscordWebhookNotifier`, selected by `Discord:NotifierMode` (`Bot` / `Webhook`). Register only the services for the selected mode. The bot notifier posts the embed, then the thread, the buttons and the ping as independent steps. (Removing webhook mode is a pending decision.)
- **Workers:** `DiscordRetryWorker` retries Failed rows and Pending rows older than 10 minutes (Abandoned after 5). `PersonalDataDeleteWorker` deletes applications older than 3 months, daily. Both run once at startup and create a DI scope per run.
- **Discord bot:**
  - `DiscordGatewayService` (hosted service) owns the socket client: subscribe events → login → start. Each event runs via `RunInBackground(label, work, logTiming)` (off the gateway task, new scope, try/catch inside), which logs "Handling {Event}" / "Handled {Event} in {ElapsedMs} ms" / "{Event} failed after {ElapsedMs} ms". The two `MessageReceived` handlers (DM relay, recruitment feed) pass `logTiming: false` because they fire for every message.
  - `DiscordInteractionRouter` (scoped) handles buttons and modals. Access is via `CanManageApplications` (any role in `RecruitmentTeamRoleIds`), checked on the click and again on submit. A button finds its application by thread ID. Modal custom IDs are `{Type}:{threadId}`.
  - Message / Accept / Decline save through `ConcurrencySaveHelper.TrySaveAsync`: on a `DbUpdateConcurrencyException` it logs a Warning, tells the officer "updated by another member of the recruitment team", and returns false so no DM, role or welcome is sent.
  - `/buttons` (`DiscordSlashCommands` + `DiscordSlashCommandsRouter`) re-posts the action buttons in an application thread. Commands are registered on every `Ready` (guild `BulkOverwrite`).
  - `ApplicantMessagingService` relays applicant DMs to the thread of the application with the latest activity (`LastRecruitmentTeamMessagedUtc ?? TimestampUtc`).
  - Constants: `DiscordCustomIds` (button IDs, never change their values, old messages use them) and `DiscordModals` (static builders + field IDs).
- **Recruitment feed:** `RecruitmentFeedHandler` filters `MessageReceived` by incoming channel → author is a webhook in `AllowedWebhookIds` → has embeds. Per embed: `WarcraftLogsEmbedParser.TryParse` (https, host `www.warcraftlogs.com`, path `/character/id/{id}`, title "Name (Class - Faction - Realm - Region)", footer "k/n Difficulty (Raid)") → `WarcraftLogsClient` query (one `encounterRankings` alias per boss, Mythic, `partition: -1`) → `FirstKillSummaryBuilder` (first kill = earliest `startTime`, best = highest `rankPercent`) → `RecruitmentFeedEmbeds.AddLogs` (code-block table, colour emoji per parse, 1,024-char field guard) → posted with `AllowedMentions.None`. Enrichment failures are logged and the original embed is still posted.
- **Database:** entity configuration in Fluent API (`Data/ApplicationConfig.cs`, applied via `ApplyConfigurationsFromAssembly`). `GuildApplication.RowVersion` is a SQL `rowversion` (`IsRowVersion()`) for optimistic concurrency. SQL connections use `EnableRetryOnFailure` (5 retries, 5 s max delay); there are no explicit transactions. If you add one (`BeginTransaction`), wrap it in `context.Database.CreateExecutionStrategy().ExecuteAsync(...)`, or EF throws because the retry strategy can't replay part of a transaction.
- **Mapping:** extension methods in `Mappings/` (no AutoMapper).
- **Auth:** cookie auth (HttpOnly, Secure, SameSite=Lax) plus Discord OAuth. The applicant's `DiscordId` and `DiscordName` come **only from the auth cookie's claims**, never from the request body. Because the cookie is Lax, the frontend and API must be same-site (subdomains of `clarityguild.co.uk`).
- **CSRF:** `OriginCheckMiddleware` rejects state-changing requests (anything except GET/HEAD/OPTIONS) whose `Origin` isn't in `Cors:AllowedOrigins`. No exclusions (the HTTP interactions endpoint was removed when the bot moved to the gateway).

## CI/CD
**Workflows** (all trigger off the workflow **name** `CI`; renaming it silently stops every deploy):
- `Dev.yaml` (`CI`): restore → build `-c Release` → test `-c Release`. On push to `main` only: `dotnet publish ClarityGuildWebsite.Api --no-build -o api-publish` → artifact `api-publish` (30 days); frontend → artifact `web-dist` (7 days).
- `deploy-api.yml` (`Deploy API`): `workflow_run` after a successful CI **push** run, or `workflow_dispatch` with input `run_id` (a CI run ID). `deploy-dev` (environment `dev`, automatic) → `deploy-prod` (environment `production`, `needs: deploy-dev`, waits for approval). Each job: download artifact → `azure/login@v3` (OIDC) → `azure/webapps-deploy@v3`. Job-level concurrency groups `deploy-api-dev` / `deploy-api-prod`, never cancelled mid-deploy.
- `azure-static-web-apps-ashy-coast-096246c0f.yml` (`Deploy frontend`): deploys `web-dist` to the production Static Web App.

**Azure / GitHub setup for API deploys (no secrets):**
- One user-assigned managed identity per environment: `id-clarity-deploy-dev`, `id-clarity-deploy-production`.
- Each has a federated credential for its GitHub environment, using **immutable subject claims**: `repo:cckraken@23668251/ClarityGuildWebsite@1361778295:environment:<dev|production>` (case-sensitive; issuer `https://token.actions.githubusercontent.com`, audience `api://AzureADTokenExchange`).
- Role: **Website Contributor scoped to that environment's App Service only**. Never resource group or subscription.
- GitHub environment variables (per environment): `AZURE_CLIENT_ID` (the identity's *Client* ID, not Object ID), `AZURE_WEBAPP_NAME` (the App Service resource name, not the URL). Repo variables: `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`. These are identifiers, not secrets.
- `production` environment: required reviewer, deployment branches `main` only.

**Troubleshooting:**
- `AADSTS700213` / `AADSTS7002138`: the federated credential's subject doesn't match. Compare it character for character with the "subject claim" line in the `azure/login` log.
- `No subscriptions found` after a successful federated login: the identity has no role assignment (or it hasn't propagated yet, allow ~10 minutes). Check from the identity's **Azure role assignments** page, which shows exactly which identity holds what. Don't fix it with `allow-no-subscriptions` or a broader scope.
- `workflow_run` always uses the deploy workflow file from `main`, so changes to it can't be tested on a branch.

## Conventions
- DTOs for all API input and output; never expose EF entities. Input DTOs contain only fields the applicant types.
- Validate on the API, not just the frontend (data annotations on the API, Zod on the web).
- Keep controllers thin; logic goes in services and mappers.
- Fail fast on missing configuration: throw with a message naming the key.
- Logging uses message templates (`"Blocked {Path}", path`), never `$""` interpolation. Never log personal data or message/DM content; log `PublicId`s.
- Any new state-changing endpoint: decide auth, Origin check and rate limiting deliberately.
- Schema changes: always a new migration (never edit one that has run anywhere). **Read every generated migration**: any `Drop…`/`Rename…` needs a deliberate reason. A bad migration that hasn't been applied anywhere: `dotnet ef migrations remove` and regenerate, don't fix forward. Backfill when adding non-null columns. Apply migrations to Azure before deploying code that needs them.
- Concurrency: status checks ("already accepted") guard against repeats one after another; `RowVersion` guards against two officers acting at the same moment. Keep both.
- Logging levels: `Logging:LogLevel` in `appsettings.json` (Default Information; `Microsoft.AspNetCore` and `Microsoft.EntityFrameworkCore.Database.Command` Warning; `Discord.Net` Information via its own logger category). Everything at Information+ reaches Application Insights, so don't log per-message noise at Information.
- Side effects: do the irreversible step last (save to the database, then DM/post).
- Discord: respond to every interaction within 3 seconds. Modals must be the first response (can't defer); modal submits defer first, then follow up on every exit. User-written text is posted with `AllowedMentions.None`.
- Tailwind: mobile-first; `gap-*` instead of margins between siblings; `min-w-0` on flex children holding inputs or long text.

### Before merging (merging to `main` deploys the API to dev automatically)
- [ ] CI green on the PR. If the PR changes `Dev.yaml` publish/upload steps, run them locally first (they only run on push).
- [ ] Migrations: if the PR adds any, generate the idempotent script (`dotnet ef migrations script --idempotent -o <file>.sql`, don't commit it), review it, and apply it to **dev** before merging.
- [ ] New config keys: added to the App Service settings (secrets) or `appsettings.json` (non-secret) for both environments.
- [ ] New Discord intents or permissions: enabled on **both** bots' portal pages (Bot → Privileged Gateway Intents) and in the servers.

### Before approving `deploy-prod`
- [ ] `deploy-dev` succeeded and the checks below pass on dev.
- [ ] Migrations applied to **production** (same reviewed script).
- [ ] Production config keys and Discord intents in place.

### After deploying (check in this order)
1. **Startup:** Log stream shows `Starting ClarityGuildWebsite.Api 1.0.0+<hash>` and the hash matches the merge commit.
2. **Gateway:** Discord.Net reaches `Ready` (no "disallowed intents" or login errors).
3. **Website:** submit a test application → embed, thread, buttons and the recruitment team ping appear.
4. **Buttons:** click **Message Applicant** → the modal opens (then close it, or send a test message).
5. **Modal submit:** Decline (or Accept) the test application → DM sent, thread record posted, status saved.
6. **Slash commands:** `/buttons` in an application thread → buttons re-posted.
7. **DM relay:** reply to the bot by DM → the message appears in the application thread with ✅.
8. **Recruitment feed:** the next Warcraft Logs post appears in the officers' channel with the logs field (production can't be tested with the test webhook, by design).
9. **Errors:** after ~5 minutes, check Application Insights:
   ```kusto
   union traces, exceptions
   | where timestamp > ago(30m)
   | where severityLevel >= 2 or itemType == "exception"
   | order by timestamp desc
   ```
10. Delete the test application (or let the retention worker remove it).

### If something breaks
- Intent/permission problem: fix it in the Discord portal or server; no redeploy needed.
- Code problem: Actions → **Deploy API** → Run workflow with `run_id` = the last good CI run's ID (from its URL). That redeploys its exact artifact to dev, then prod after approval. Artifacts last 30 days; older than that, revert the commit on `main` instead.
- Database problem: don't roll back a migration on production without a plan; most migrations are additive and the old code still works with them.

## Gotchas
- Config arrays: read with `GetSection("X").Get<T[]>()`. `GetValue<...>` returns null/0 for arrays. Arrays merge by index across config sources.
- App Service setting names use `__` for `:` (e.g. `Cors__AllowedOrigins__0`). Changing settings restarts the app.
- App Service needs `WEBSITE_RUN_FROM_PACKAGE=1` and `SCM_DO_BUILD_DURING_DEPLOYMENT=false`. If the log shows `hostingstart.dll`, `wwwroot` is empty: check those settings and redeploy.
- The gateway only works with Always On and a single instance; a deploy briefly disconnects the bot.
- A Discord app receives interactions over the gateway **or** an Interactions Endpoint URL, never both. Keep the portal URL empty.
- `GetRequiredService<T>` must ask for exactly the registered type (interface vs class). It fails only at runtime, and inside `RunInBackground` only as a logged error.
- Event handlers run in parallel, so "already accepted/declined" checks alone can race; `RowVersion` + `ConcurrencySaveHelper` catch the simultaneous case.
- EF skips the `UPDATE` entirely when nothing changed, so a no-op save never raises a concurrency conflict.
- The gateway misses messages posted while the bot is offline (e.g. during a deploy); the recruitment feed has no catch-up yet. `Ready` fires on every reconnect, not just once.
- WCL: a `200 OK` GraphQL response can still contain an `errors` array, and `encounterRankings` returns a JSON scalar: check `JsonValueKind` before `TryGetProperty`.
- `All()` on an empty collection returns true; validate "not empty" separately in options validation.
- Application Insights SDK 3.x sends Information logs by default (2.x only sent Warning+); tutorials written for 2.x are wrong about this.
- Interpolating a collection (`$"{list}"`, `.ToString()`) prints its type name. Use `string.Join` + `Select`.
- Discord limits: modal title and labels 45 chars, message 2,000, embed description 4,096. A bot can only DM users who share a server with it and allow DMs (error 50007).
- The SWA deploy workflow file must keep its Azure-generated name (`azure-static-web-apps-ashy-coast-096246c0f.yml`), and both deploy workflows follow the workflow **name** `CI` via `workflow_run`. Renaming either one breaks deploys.
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
