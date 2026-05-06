using System.IO.Pipelines;
using System.Net;

namespace Velocity.Service.Transport;

/// <summary>
/// Defines a contract for managing a transport connection,
/// including access to a connected remote endpoint and a data stream for communication.
/// </summary>
public interface ITransportConnection : IAsyncDisposable
{
    EndPoint? RemoteEndPoint { get; }

    PipeReader Input { get; }

    PipeWriter Output { get; }
}