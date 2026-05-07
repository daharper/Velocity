using Microsoft.Extensions.Options;
using Velocity.Service.Networking;

namespace Velocity.Service.Transport;

/// <summary>
/// Represents a transport connector that establishes TCP-based connections.
/// </summary>
public sealed class TcpTransportConnector(
    TcpConnector tcpConnector,
    IOptions<TcpClientOptions> options) : ITransportConnector
{
    private readonly TcpClientOptions _options = options.Value;

    /// <summary>
    /// Establishes a transport connection asynchronously to the configured host and port.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// Upon completion, returns an instance of <see cref="ITransportConnection"/>.
    /// </returns>
    public async ValueTask<ITransportConnection> ConnectAsync(CancellationToken cancellationToken)
    {
        var socket = await tcpConnector.ConnectAsync(
            _options.Host,
            _options.Port,
            cancellationToken);

        return new SocketTransportConnection(socket, _options);
    }
}