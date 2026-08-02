using System.Text.Json;

namespace EncryptedDbAtRest.Server;

public static class JsonSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = false,
    };
    
    public static string Serialize<T>(T instance)
    {
        return System.Text.Json.JsonSerializer.Serialize(instance, DefaultOptions);
    }
}