using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EncryptedDbAtRest.Server;

[JsonDerivedType(typeof(Customer))]
[JsonDerivedType(typeof(TimeBlock))]
public abstract class Instance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "varchar(32)")]
    public string Id { get; init; }

    [Required]
    
    public string Data { get; private set; } = null!;
    
    [Required]
    public DateTime Created { get; init; }
    
    [Required]
    public required Tenant Tenant { get; init; }

    internal string? Nonce { get; set; }
    
    public void SetData()
    {
       Data = this.GetEncryptedData();
    }

    public Instance(Tenant tenant)
    {
        Id= Guid.NewGuid().ToString().Replace("-", "");
        Created = DateTime.UtcNow;
        Tenant = tenant;
    }
}

public static class InstanceExtensions
{
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";

    public static string GetEncryptedData<T>(this T instance) where T : Instance
    {
        var nonce = "";
        var rnd = new Random();
        for (int i = 0; i < 16; i++)
        {
            nonce += AllowedChars[rnd.Next(0, AllowedChars.Length)];
        }

        instance.Nonce = nonce;
        return JsonSerializer.Serialize(instance);
    }
}