using System.Net;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Velocity.Service.Networking;

public sealed record DnsCacheEntry(string[] Addresses);

/// <summary>
/// Provides functionality for resolving DNS hostnames to IP addresses.
/// </summary>
public sealed class DnsResolver(
    HybridCache cache,
    ILogger<DnsResolver> logger,
    IOptions<TcpClientOptions> options) : IDnsResolver
{
    private readonly TcpClientOptions _clientOptions = options.Value;

    /// <summary>
    /// Resolves the specified DNS hostname to an array of IP addresses asynchronously.
    /// </summary>
    /// <param name="host">The hostname to resolve.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A task for an array of <see cref="IPAddress"/> objects that represent the resolved IP addresses.</returns>
    public async ValueTask<IPAddress[]> ResolveAsync(string host, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);

        var normalizedHost = NormalizeHost(host);

        if (_clientOptions.DnsCacheDuration <= TimeSpan.Zero)
        {
            var cacheEntry = await ResolveUncachedAsync(
                normalizedHost,
                cancellationToken);

            return ToIpAddresses(cacheEntry);
        }
        
        var cacheOptions = new HybridCacheEntryOptions
        {
            Expiration = _clientOptions.DnsCacheDuration,
            LocalCacheExpiration = _clientOptions.DnsCacheDuration
        };

        var entry = await cache.GetOrCreateAsync(
            $"dns:{normalizedHost}",
            async token => await ResolveUncachedAsync(normalizedHost, token),
            cacheOptions,
            cancellationToken: cancellationToken);
        
        return entry.Addresses.Select(IPAddress.Parse).ToArray();
    }
    
    // Resolves the specified DNS hostname to an array of IP addresses asynchronously, bypassing any caching mechanism.
    private async ValueTask<DnsCacheEntry> ResolveUncachedAsync(string host, CancellationToken cancellationToken)
    {
        var started = TimeProvider.System.GetTimestamp();
        var addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
        var elapsed = TimeProvider.System.GetElapsedTime(started);

        logger.LogInformation(
            "Resolved {Host} to {AddressCount} address(es) in {ElapsedMs} ms",
            host,
            addresses.Length,
            elapsed.TotalMilliseconds);

        return new DnsCacheEntry(addresses.Select(static a => a.ToString()).ToArray());
    }

    // Converts a DNS cache entry into an array of IPAddress objects.
    private static IPAddress[] ToIpAddresses(DnsCacheEntry entry)
        => entry.Addresses.Select(IPAddress.Parse).ToArray();
    
    // Normalizes the specified DNS hostname.
    private static string NormalizeHost(string host)
        => host.Trim().TrimEnd('.').ToLowerInvariant();
}