using EncryptedDbAtRest.Server.Encryption;
using Microsoft.AspNetCore.Mvc;

namespace EncryptedDbAtRest.Server;

public static class InstanceAccess
{

    public static Instance? Get([FromServices] DbContext dbContext)
    {
        return dbContext.Customers.FirstOrDefault();
    }
    
    public static void TestDecrypt([FromQuery] string str)
    {
        var encrypted = SymmetricEncryption.GetInstance().Encrypt(ref str);
        Console.WriteLine("Encrypted value: " + encrypted);

        var decrypted = SymmetricEncryption.GetInstance().Decrypt(ref encrypted);
        Console.WriteLine("Decrypted value: " + decrypted);
    }

    public static void Create([FromServices] DbContext dbContext, HttpContext httpContext)
    {
        httpContext.Request.Cookies.TryGetValue("EncryptedData", out var encryptedData);
        var cookie = System.Text.Json.JsonSerializer.Deserialize<CookieConfiguration>(encryptedData);
        dbContext.Customers.Add(new Customer("a", "b", "c", "d"));
        dbContext.SaveChanges();
    }
}