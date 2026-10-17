using ClarityGuildWebsite.Api.Data;
using ClarityGuildWebsite.Api.Services;
using Discord.Rest;
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

builder.Services.AddHostedService<DiscordRetryWorker>();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("appPostPolicy", HttpContext =>

        RateLimitPartition.GetTokenBucketLimiter(
            partitionKey:
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
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
builder.Services.AddHttpClient<IApplicationNotifier, DiscordWebhookNotifier>();
builder.Services.AddScoped<IApplicationDiscordPostingService, ApplicationDiscordPostingService>();
builder.Services.AddSingleton(new DiscordRestClient());
var notifierMode = builder.Configuration["Discord:NotifierMode"]
    ?? throw new InvalidOperationException("Discord:NotifierMode is not configured.");
if (notifierMode == "Bot")
{
    builder.Services.AddSingleton (new DiscordRestClient());
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
              .AllowAnyMethod();
    });
});

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
app.UseRateLimiter();
app.MapControllers();


app.Run();

