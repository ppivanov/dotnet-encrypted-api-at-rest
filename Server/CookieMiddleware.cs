namespace EncryptedDbAtRest.Server;

public class CookieMiddleware
{
    private readonly RequestDelegate _next;

    public CookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, DbContext dbContext)
    {
        Console.WriteLine($"Incoming: {httpContext.Request.Method} {httpContext.Request.Path}");
        if (httpContext.Request.Path == "api/login")
        {
            var requestBody = httpContext.ReadRequestBody<LoginRequest>();

        }
        else
        {
            var cookie = httpContext.Request.Cookies.FirstOrDefault(c => c.Key == "sessionId");
        }
        //var sessionId = dbContext.Sessions.
        await _next(httpContext);
        Console.WriteLine($"Outgoing: {httpContext.Response.StatusCode}");
    }
}

public static class CookieMiddlewareExtensions
{
    public static IApplicationBuilder UseCookieSessions(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CookieMiddleware>();
    }
}
