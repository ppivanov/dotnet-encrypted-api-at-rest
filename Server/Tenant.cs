using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json;

namespace EncryptedDbAtRest.Server;

public class Tenant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "varchar(32)")]
    public string Id { get; private set; } = null!;

    [Required]
    public string Name { get; private set; } = null!;

    [NotMapped]
    private TenantEncryption _encryption = null!;

    public Tenant()
    {

    }

    public Tenant(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public void Initialize()
    {
        _encryption = new(this);
    }

    public void PrepareInstance(Instance instance)
    {
        _encryption.Encrypt(instance);
    }
}

public class TenantEncryption
{
    private byte[]? EncryptionKey { get; set; }

    private byte[]? EncryptionIV { get; set; }

    private Tenant Tenant { get; set; }

    private Aes Aes { get; set; }

    private string KeyPath => $"{Tenant.Id}.key";

    public TenantEncryption(Tenant tenant)
    {
        Tenant = tenant;
        Aes = Aes.Create();

        Initialize();
    }

    public void Initialize()
    {
        if (EncryptionKey?.Length > 0)
        {
            Aes.Key = EncryptionKey;
        }
        else
        {
            Aes.GenerateKey();
            Env.WriteBytesToFilepathVariable(KeyPath, Aes.Key);
        }

        if (EncryptionIV?.Length > 0)
        {
            Aes.IV = EncryptionIV;
        }
        else
        {
            Aes.GenerateIV();
            Env.WriteBytesToFilepathVariable(KeyPath, Aes.IV);
        }
    }

    public void Encrypt(Instance instance)
    {
        var encryptor = Aes.CreateEncryptor();

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(instance.SerializeToJson());

        instance.SetData(Convert.ToBase64String(ms.ToArray()));
    }

    public Instance Decrypt(Instance instance)
    {
        if (instance == null)
        {
            throw new Exception("Attempt to deserialize null instance");
        }

        var jsonData = Decypher(instance);
        using var jsonDoc = JsonDocument.Parse(jsonData);
        foreach (var jsonElement in jsonDoc.RootElement.EnumerateObject())
        {
            var instanceProps = instance.GetType().GetProperties();
            foreach (var prop in instanceProps)
            {
                if (prop.CustomAttributes.Any(ca => ca.GetType() == typeof(NotMappedAttribute))
                        && prop.Name == jsonElement.Name)
                {
                    var value = jsonElement.Value.Deserialize(prop.PropertyType);
                    prop.SetValue(instance, value);
                }
            }
        }

        return instance;
    }

    private string Decypher(Instance instance)
    {
        var decryptor = _aes.CreateDecryptor();

        using var ms = new MemoryStream(Convert.FromBase64String(instance.Data));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}