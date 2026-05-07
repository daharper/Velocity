using Microsoft.Extensions.Caching.Hybrid;

namespace Velocity.Service.Identity;

public sealed class AuthorizationCache(HybridCache cache, IAuthorizationProvider provider)
{
    public async ValueTask<AuthorizationProfile> GetAsync(string jid, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jid);

        var normalizedJid = NormalizeSubject(jid);
        var cacheKey = $"identity:authorization:{normalizedJid}";

        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };

        return await cache.GetOrCreateAsync(
            cacheKey,
            async token => await provider.GetAuthorizationAsync(normalizedJid, token),
            options,
            cancellationToken: cancellationToken);
    }

    private static string NormalizeSubject(string jid)
        => jid.Trim().ToLowerInvariant();
}