namespace Velocity.Service.Transport;

/// <summary>
/// Defines the contract for creating and managing transport connections in a service.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for establishing connections to a specific
/// transport medium, such as TCP or other communication protocols. The <see cref="ConnectAsync"/>
/// method provides an asynchronous mechanism to initiate and obtain the connection.
/// </remarks>
public interface ITransportConnector
{
    ValueTask<ITransportConnection> ConnectAsync(CancellationToken cancellationToken);
}