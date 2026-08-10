using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EncryptedDbAtRest.Server;

[JsonDerivedType(typeof(Customer))]
public abstract class Instance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "varchar(32)")]
    public string Id { get; private set; }

    [Required]

    public string Data { get; private set; } = null!;

    [Required]
    public DateTime Created { get; private set; }

    [Required]
    public Tenant Tenant { get; private set; }

    public void SetData(string data)
    {
        Data = data;
    }

    public Instance(Tenant tenant)
    {
        Id = StringUtils.GetNewId();
        Created = DateTime.UtcNow;
        Tenant = tenant;
    }
}
