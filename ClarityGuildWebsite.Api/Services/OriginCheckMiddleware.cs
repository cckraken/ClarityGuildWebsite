public class OriginCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] _allowedOrigins;
    private readonly ILogger _logger;

    public OriginCheckMiddleware(RequestDelegate next, IConfiguration config, ILogger<OriginCheckMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();

        if (allowedOrigins is null || allowedOrigins.Length == 0)
            throw new InvalidOperationException("Cors:AllowedOrigins is missing or empty.");

        _allowedOrigins = allowedOrigins;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method))
        {
            await _next(context);
            return;
        }

        var origin = context.Request.Headers.Origin.ToString();

        if (string.IsNullOrEmpty(origin) || !_allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            _logger.LogWarning("Blocked {Method} {Path} from origin {Origin}", method, context.Request.Path, origin);
            return;
        }

        await _next(context);
    }
}

