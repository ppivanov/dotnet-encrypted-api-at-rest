namespace EncryptedDbAtRest.Server;

public static class StringUtils
{
    public static string GetNewId()
    {
        return Guid.NewGuid()
            .ToString()
            .Replace("-", "");
    }
}
