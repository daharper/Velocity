using System.Net;

namespace Velocity.Service.Networking;

/// <summary>
/// Represents a contract for resolving a DNS hostname to its associated IP addresses.
/// </summary>
public interface IDnsResolver
{
    /// <summary>
    /// Resolves a DNS hostname into its associated IP addresses asynchronously.
    /// </summary>
    /// <param name="host">The DNS hostname to resolve.</param>
    /// <param name="cancellationToken">A token that can be used to observe cancellation requests.</param>
    /// <returns>
    /// A task for an array of <see cref="IPAddress"/> objects associated with the specified hostname.
    /// </returns>
    ValueTask<IPAddress[]> ResolveAsync(string host, CancellationToken cancellationToken);
}