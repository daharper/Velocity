namespace Velocity.Service.Transport;

/// <summary>
/// Defines the contract for creating and managing transport connections in a service.
/// </summary>
public interface ITransportConnector
{
    /// <summary>
    /// Establishes a transport connection asynchronously to a remote endpoint.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation containing an <see cref="ITransportConnection"/>
    /// </returns>
    ValueTask<ITransportConnection> ConnectAsync(CancellationToken cancellationToken);
}