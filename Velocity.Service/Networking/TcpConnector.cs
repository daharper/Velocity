using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Velocity.Service.Networking;

/// <summary>
/// Provides functionality to initiate TCP connections to a specified host and port.
/// </summary>
public sealed class TcpConnector(
    IDnsResolver dnsResolver,
    IAddressSelector addressSelector,
    ILogger<TcpConnector> logger,
    IOptions<TcpClientOptions> options)
{
    private readonly TcpClientOptions _clientOptions = options.Value;

    /// <summary>
    /// Establishes an asynchronous TCP connection to the specified host and port.
    /// </summary>
    /// <param name="host">The hostname or IP address of the remote endpoint to connect to.</param>
    /// <param name="port">The port number of the remote endpoint to connect to.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the connection attempt.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="Socket"/> result.</returns>
    public async ValueTask<Socket> ConnectAsync(string host, int port, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);

        var resolvedAddresses = await dnsResolver.ResolveAsync(host, cancellationToken);
        var addresses = addressSelector.Select(resolvedAddresses, _clientOptions.AddressFamilyPreference);

        if (addresses.Count == 0)
            throw new SocketException((int)SocketError.HostNotFound);
        
        List<Exception> failures = [];

        foreach (var address in addresses)
        {
            Socket? socket = null;

            try
            {
                socket = CreateSocket(address);

                using var timeoutCts = new CancellationTokenSource(_clientOptions.ConnectTimeout);

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                        timeoutCts.Token,
                        cancellationToken);

                IPEndPoint endpoint = new(address, port);

                var started = Stopwatch.GetTimestamp();

                logger.LogDebug(
                    "Connecting to {Host}:{Port} via {Address} ({Family})",
                    host,
                    port,
                    address,
                    address.AddressFamily);

                await socket.ConnectAsync(endpoint, linkedCts.Token);

                var elapsed = Stopwatch.GetElapsedTime(started);

                logger.LogInformation(
                    "Connected to {Host}:{Port} via {Address} ({Family}) in {ElapsedMs} ms",
                    host,
                    port,
                    address,
                    address.AddressFamily,
                    elapsed.TotalMilliseconds);

                return socket;
            }
            catch (Exception ex) when (IsConnectFailure(ex, cancellationToken))
            {
                socket?.Dispose();
                failures.Add(ex);
                
                logger.LogWarning(ex, "Failed to connect to {Host}:{Port} via {Address}", host, port, address);
            }
        }

        throw new AggregateException($"Failed to connect to {host}:{port}.", failures);
    }
    
    // Creates a new instance of a <see cref="Socket"/> configured for TCP connections using the specified IP address.
    private static Socket CreateSocket(IPAddress address)
        => new(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
    
    // determines if the exception indicates a connection failure
    private static bool IsConnectFailure(Exception exception, CancellationToken outerCancellationToken)
    {
        if (outerCancellationToken.IsCancellationRequested) return false;
        return exception is SocketException or OperationCanceledException or TimeoutException;
    }
}
