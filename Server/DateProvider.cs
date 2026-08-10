namespace EncryptedDbAtRest.Server;

public interface IDateProvider
{
    DateTime UtcNow { get; }
}

public static class DateProvider
{
    private class DefaultDateProvider : IDateProvider
    {
        DateTime IDateProvider.UtcNow => DateTime.UtcNow;
    }

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
