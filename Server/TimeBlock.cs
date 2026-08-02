using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EncryptedDbAtRest.Server;

public class TimeBlock : Instance
{
    [NotMapped]
    public DateTime Start { get; init; }

    [NotMapped]
    public DateTime End { get; init; }

    public TimeBlock() { }

    public TimeBlock(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
}