using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        await using var connection = await transport.ConnectAsync(cancellationToken);

        await handler.HandleAsync(connection, cancellationToken);
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