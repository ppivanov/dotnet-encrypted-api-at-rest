using System.ComponentModel.DataAnnotations.Schema;

namespace EncryptedDbAtRest.Server;

public class LoginHistory : Instance
{
    [NotMapped]
    public string UserId { get; private set; }

    [NotMapped]
    public string SessionId { get; private set; }

    public LoginHistory(Tenant tenant) : base(tenant)
    {
    }
}
