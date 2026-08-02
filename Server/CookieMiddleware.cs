namespace EncryptedDbAtRest.Server;

public class CookieMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, DbContext dbContext)
    {
        Console.WriteLine($"Incoming: {httpContext.Request.Method} {httpContext.Request.Path}");
        if (httpContext.Request.Path == "api/login")
        {
            httpContext.Response.Cookie();
        }
        var sessionId = dbContext.Sessions.
        await _next(httpContext);
        Console.WriteLine($"Outgoing: {httpContext.Response.StatusCode}");
    }
}

public static class CookieMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCulture(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CookieMiddleware>();
    }
}
