using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.Middleware;
using ClarityGuildWebsite.Api.Services;
using Discord.Rest;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction())
{
    builder.Services.AddApplicationInsightsTelemetry();
}
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? throw new InvalidOperationException("Cors:AllowedOrigins is not configured.");

var discordClientId = builder.Configuration["Discord:ClientId"] ?? throw new InvalidOperationException("Discord:ClientId is not configured.");
var discordClientSecret = builder.Configuration["Discord:ClientSecret"] ?? throw new InvalidOperationException("Discord:ClientSecret is not configured.");

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
    })
    .AddDiscord(options =>
    {
        options.ClientId = discordClientId;
        options.ClientSecret = discordClientSecret;
        options.Scope.Add("guilds.members.read");
    });

builder.Services.AddHostedService<DiscordRetryWorker>();
builder.Services.AddHostedService<PersonalDataDeleteWorker>();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("appPostPolicy", httpContext =>

        RateLimitPartition.GetTokenBucketLimiter(
            partitionKey:
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 3,
                QueueLimit = 3,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                TokensPerPeriod = 3,
                AutoReplenishment = true
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ClarityDb")
    ?? throw new InvalidOperationException("Connection string 'ClarityDb' is not configured.")));
builder.Services.AddScoped<IApplicationDiscordPostingService, ApplicationDiscordPostingService>();
var notifierMode = builder.Configuration["Discord:NotifierMode"]
    ?? throw new InvalidOperationException("Discord:NotifierMode is not configured.");
if (notifierMode == "Bot")
{
    builder.Services.AddSingleton<DiscordRestClient>();
    builder.Services.AddSingleton<DiscordBotConnectionService>();
    builder.Services.AddScoped<IApplicationNotifier, DiscordBotNotifier>();
}
else if (notifierMode == "Webhook")
{
    builder.Services.AddHttpClient<IApplicationNotifier, DiscordWebhookNotifier>();
}
else
{
    throw new InvalidOperationException($"Discord:NotifierMode '{notifierMode}' is invalid. Expected 'Bot' or 'Webhook'.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthorization();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseMiddleware<OriginCheckMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();


app.Run();

