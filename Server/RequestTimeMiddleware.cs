namespace EncryptedDbAtRest.Server;

public class RequestTimeMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTimeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<RequestTimeMiddleware> logger)
    {
        var startTime = DateProvider.UtcNow;
        logger.LogInformation("Incoming {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context);

        var elapsed = DateProvider.UtcNow - startTime;
        logger.LogInformation("Completed {Method} {Path} with {StatusCode} in {Elapsed}ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsed.TotalMilliseconds);
    }
}

public static class RequestTimeMiddlewareExtensions
{
    public static IApplicationBuilder UseTimeTracking(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestTimeMiddleware>();
    }
}