using System.ComponentModel.DataAnnotations.Schema;

namespace EncryptedDbAtRest.Server;

public class User : Instance
{
    [NotMapped]
    public string Username { get; set; }

    [NotMapped]
    public string Password { get; set; }

    public User() : base(null)
    {

    }

    public User(Tenant tenant) : base(tenant)
    {
    }
}
