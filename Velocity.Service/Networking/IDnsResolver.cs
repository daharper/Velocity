using System.Net;

namespace Velocity.Service.Networking;

public interface IDnsResolver
{
    ValueTask<IPAddress[]> ResolveAsync(string host, CancellationToken cancellationToken);
}