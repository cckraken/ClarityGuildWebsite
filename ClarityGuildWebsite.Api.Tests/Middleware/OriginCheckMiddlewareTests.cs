using ClarityGuildWebsite.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClarityGuildWebsite.Api.Tests.Middleware;

public class OriginCheckMiddlewareTests
{
    private const string AllowedOrigin = "https://localhost:5173";

    // ---------- Helpers ----------

    /// <summary>Records whether the next middleware in the pipeline was called.</summary>
    private sealed class NextSpy
    {
        public bool Called { get; set; }
    }

    private static IConfiguration BuildConfig(params string[] allowedOrigins)
    {
        var settings = new Dictionary<string, string?>();
        for (var i = 0; i < allowedOrigins.Length; i++)
        {
            settings[$"Cors:AllowedOrigins:{i}"] = allowedOrigins[i];
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    private static (OriginCheckMiddleware middleware, NextSpy next) CreateMiddleware()
    {
        var spy = new NextSpy();
        var middleware = new OriginCheckMiddleware(
            _ => { spy.Called = true; return Task.CompletedTask; },
            BuildConfig(AllowedOrigin),
            NullLogger<OriginCheckMiddleware>.Instance);

        return (middleware, spy);
    }

    private static DefaultHttpContext CreateRequest(string method, string? origin)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        if (origin is not null)
        {
            context.Request.Headers.Origin = origin;
        }
        return context;
    }

    // ---------- Blocked requests ----------

    [Fact]
    public async Task InvokeAsync_PostFromUnknownOrigin_Returns403AndSkipsNext()
    {
        var (middleware, next) = CreateMiddleware();
        var context = CreateRequest("POST", "https://evil.com");

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.False(next.Called);
    }

    [Fact]
    public async Task InvokeAsync_PostWithNoOriginHeader_Returns403AndSkipsNext()
    {
        var (middleware, next) = CreateMiddleware();
        var context = CreateRequest("POST", origin: null);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.False(next.Called);
    }

    [Fact]
    public async Task InvokeAsync_PostFromLookalikeOrigin_Returns403AndSkipsNext()
    {
        // Same host and port but a different scheme is a different origin.
        var (middleware, next) = CreateMiddleware();
        var context = CreateRequest("POST", "http://localhost:5173");

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.False(next.Called);
    }

    [Fact]
    public async Task InvokeAsync_PostToLookalikeInteractionsPath_Returns403AndSkipsNext()
    {
        var (middleware, next) = CreateMiddleware();
        var context = CreateRequest("POST", null);
        context.Request.Path = "/api/discord/interactions-evil";
        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.False(next.Called);
    }

    [Fact]
    public async Task InvokeAsync_PostToDoubleSlashInteractionsPath_Returns403AndSkipsNext()
    {
        var (middleware, next) = CreateMiddleware();
        var context = CreateRequest("POST", null);
        context.Request.Path = "//api/discord/interactions";
        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.False(next.Called);
    }

    // ---------- Allowed requests ----------

    [Fact]
    public async Task InvokeAsync_PostFromAllowedOrigin_CallsNext()
    {
        var (middleware, next) = CreateMiddleware();
        var context = CreateRequest("POST", AllowedOrigin);

        await middleware.InvokeAsync(context);

        Assert.True(next.Called);
    }

    [Fact]
    public async Task InvokeAsync_PostOriginDiffersOnlyByCase_CallsNext()
    {
        var (middleware, next) = CreateMiddleware();
        var context = CreateRequest("POST", "HTTPS://LOCALHOST:5173");

        await middleware.InvokeAsync(context);

        Assert.True(next.Called);
    }

    [Fact]
    public async Task InvokeAsync_GetFromUnknownOrigin_CallsNext()
    {
        // Safe methods pass through: GET shouldn't change state.
        var (middleware, next) = CreateMiddleware();
        var context = CreateRequest("GET", "https://evil.com");

        await middleware.InvokeAsync(context);

        Assert.True(next.Called);
    }

    // ---------- Configuration ----------

    [Fact]
    public void Constructor_NoAllowedOriginsConfigured_Throws()
    {
        var emptyConfig = BuildConfig();

        Assert.Throws<InvalidOperationException>(() => new OriginCheckMiddleware(
            _ => Task.CompletedTask,
            emptyConfig,
            NullLogger<OriginCheckMiddleware>.Instance));
    }
}