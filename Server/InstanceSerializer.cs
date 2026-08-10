using EncryptedDbAtRest.Server.Encryption;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace EncryptedDbAtRest.Server;

public static class InstanceSerializer
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = false,
    };

    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";

    public static void Serialize<T>(this T instance) where T : Instance
    {
        var json = instance.SerializeToJson();
        Console.WriteLine(json);
        instance.SetData(SymmetricEncryption.Instance.Encrypt(ref json));
    }

    private static string SerializeToJson<T>(this T instance) where T : Instance
    {
        return JsonSerializer.Serialize(instance, DefaultOptions);
    }

    public static T Deserialize<T>(this T? instance) where T : Instance
    {
        if (instance == null)
        {
            throw new Exception("Attempt to deserialize null instance");
        }
        //var json = JsonSerializer
        //    .Deserialize<T>(instance.Data, DefaultOptions)
        //    ?? throw new Exception("Failed to deserialize JSON");

        var encryptedData = instance.Data;
        var jsonData = SymmetricEncryption.Instance.Decrypt(ref encryptedData);
        using var jsonDoc = JsonDocument.Parse(jsonData);
        foreach (var jsonElement in jsonDoc.RootElement.EnumerateObject())
        {
            var instanceProps = instance.GetType().GetProperties();
            foreach (var prop in instanceProps)
            {
                if (prop.CustomAttributes.Any(ca => ca.GetType() == typeof(NotMappedAttribute))
                        && prop.Name == jsonElement.Name)
                {
                    var value = jsonElement.Value.Deserialize(prop.PropertyType, DefaultOptions);
                    prop.SetValue(instance, value);
                }
            }
        }

        return instance;
    }
}
