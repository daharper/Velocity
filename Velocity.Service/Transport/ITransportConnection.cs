using System.IO.Pipelines;
using System.Net;

namespace Velocity.Service.Transport;

/// <summary>
/// Defines a contract for managing a transport connection.
/// </summary>
public interface ITransportConnection : IAsyncDisposable
{
    /// <summary>
    /// Gets the remote network endpoint of the connected transport channel.
    /// </summary>
    EndPoint? RemoteEndPoint { get; }

    /// <summary>
    /// Provides a <see cref="PipeReader"/> for reading inbound data from the transport connection.
    /// </summary>
    PipeReader Input { get; }

    /// <summary>
    /// Provides a writable stream for outgoing data in the transport connection.
    /// </summary>
    PipeWriter Output { get; }
}