using Microsoft.Extensions.Caching.Memory;

namespace EncryptedDbAtRest.Server;

/// <summary>
/// Authenticated session. It has a <see cref="SessionCacheExtensions.MillisecondsAbsoluteExpiration">default expiration</see>
/// </summary>
public class Session
{
    public required string Id { get; set; }

    public required User User { get; set; }
}

public static class SessionCacheExtensions
{
    private const int MillisecondsAbsoluteExpiration = 10 * 60 * 1000; // 10 minutes
    private static readonly MemoryCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow =
            TimeSpan.FromMilliseconds(MillisecondsAbsoluteExpiration)
    };

    public static void MaintainSession(this IMemoryCache sessionCache, string sessionId, User user)
    {
        if (sessionCache.TryGetValue(sessionId, out Session? currentSession) && currentSession != null)
        {
            sessionCache.ExtendSession(currentSession);
        }
        else
        {
            sessionCache.CreateSession(sessionId, user);
        }
    }

    public static void ClearSession(this IMemoryCache sessionsCache, string sessionId)
    {
        sessionsCache.Remove(sessionId);
    }

    public static void CreateSession(this IMemoryCache sessionsCache, string sessionId, User user)
    {
        sessionsCache.CreateSession(new() { Id = sessionId, User = user });
    }

    public static void CreateSession(this IMemoryCache sessionsCache, Session session)
    {
        sessionsCache.Set(
            session.Id,
            session,
            _cacheOptions);
    }

    public static void ExtendSession(this IMemoryCache sessionsCache, Session currentSession)
    {
        sessionsCache.Remove(currentSession.Id);
        sessionsCache.CreateSession(currentSession);
    }
}