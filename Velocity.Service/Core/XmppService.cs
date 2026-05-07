using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Velocity.Service.Networking;
using Velocity.Service.Transport;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Core;

/// <summary>
/// Represents an XMPP service.
/// </summary>
public sealed class XmppService(IHost host) : IAsyncDisposable
{
    /// <summary>
    /// Creates a new instance of <see cref="XmppServiceBuilder"/> initialized with the specified arguments.
    /// </summary>
    /// <param name="args">An array of command-line arguments to configure the XMPP service.</param>
    /// <returns>A new instance of <see cref="XmppServiceBuilder"/> configured with the specified arguments.</returns>
    public static XmppServiceBuilder CreateBuilder(string[] args) => new(args);

    /// <summary>
    /// Executes the XMPP service by starting the host and managing the lifecycle of the transport connection.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await host.StartAsync(cancellationToken);

        var transport = host.Services.GetRequiredService<ITransportConnector>();
        var handler = host.Services.GetRequiredService<IXmppConnectionHandler>();
        var logger = host.Services.GetRequiredService<ILogger<XmppService>>();
        var options = host.Services.GetRequiredService<IOptions<TcpClientOptions>>().Value;
        
        var attempt = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                ++attempt;

                logger.LogInformation("Starting XMPP connection attempt {Attempt}", attempt);

                await using var connection = await transport.ConnectAsync(cancellationToken);

                attempt = 0;

                await handler.HandleAsync(connection, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "XMPP connection failed. Reconnecting.");

                if (options.MaxReconnectAttempts is { } max && attempt >= max)
                {
                    logger.LogError(ex, "XMPP connection failed after {Attempt} attempt(s). Stopping.", attempt);
                    throw;
                }

                await Task.Delay(options.ReconnectDelay, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Releases the unmanaged resources and disposes of the managed resources used by the XMPP service asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await host.StopAsync();
        host.Dispose();
    }
}