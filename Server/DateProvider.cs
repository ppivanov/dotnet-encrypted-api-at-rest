namespace EncryptedDbAtRest.Server;

public static class DateProvider
{
    private static readonly IDateProvider DefaultImplementation = new DefaultDateProvider();
    private static IDateProvider Implementation { get; set; } = DefaultImplementation;
    public static void RevertToDefaultImplementation()
    {
        Implementation = DefaultImplementation;
    }
    
    public static DateTime UtcNow => Implementation.UtcNow;
    public static DateTime LocalDateTime => UtcNow.ToLocalTime();
    public static DateOnly LocalDate => DateOnly.FromDateTime(LocalDateTime);
}

public interface IDateProvider
{
    DateTime UtcNow { get; }
}

class DefaultDateProvider : IDateProvider
{
    DateTime IDateProvider.UtcNow => DateTime.UtcNow;
}