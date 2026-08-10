using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EncryptedDbAtRest.Server;

public class Tenant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(TypeName = "varchar(32)")]
    public string Id { get; private set; } = null!;

    [Required]
    public string Name { get; private set; } = null!;

    public Tenant()
    {

    }

    public Tenant(string id, string name)
    {
        Id = id;
        Name = name;
    }
}