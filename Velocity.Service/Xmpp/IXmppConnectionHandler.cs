using Velocity.Service.Transport;

namespace Velocity.Service.Xmpp;

/// <summary>
/// Defines a contract for handling XMPP transport connections.
/// </summary>
public interface IXmppConnectionHandler
{
    /// <summary>
    /// Handles the lifecycle of an XMPP transport connection, including reading, writing, and dispatching messages.
    /// </summary>
    /// <param name="connection">The transport connection representing the remote endpoint and communication streams.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(ITransportConnection connection, CancellationToken cancellationToken);
}