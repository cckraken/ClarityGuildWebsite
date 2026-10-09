using ClarityGuildWebsite.Api.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? throw new InvalidOperationException("Cors:AllowedOrigins is not configured.");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                TokensPerPeriod = 3,
                AutoReplenishment = true
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ClarityDb")
    ?? throw new InvalidOperationException("Connection string 'ClarityDb' is not configured.")));
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

