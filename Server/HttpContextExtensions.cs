using System.Text.Json;

namespace EncryptedDbAtRest.Server;

public static class HttpContextExtensions
{
    public static async Task<T> ReadRequestBody<T>(this HttpContext httpContext)
    {
        // Source - https://stackoverflow.com/a/51437610
        // Posted by mattinsalto, modified by community. See post 'Timeline' for change history
        // Retrieved 2026-08-07, License - CC BY-SA 4.0

        //  Enable seeking
        httpContext.Request.EnableBuffering();
        //  Read the stream as text
        var bodyAsText = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
        //  Set the position of the stream to 0 to enable rereading
        httpContext.Request.Body.Position = 0;

        return JsonSerializer.Deserialize<T>(bodyAsText) ?? throw new InvalidOperationException("Failed to deserialize request body");
    }
}
