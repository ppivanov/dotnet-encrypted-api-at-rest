using System.ComponentModel.DataAnnotations;

namespace EncryptedDbAtRest.Server;

public class Key : Instance
{
    [Required]
    public string KeyValue { get; set; }

    public string Get()
    {
        return "";
    }
}