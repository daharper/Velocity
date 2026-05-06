using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;

namespace Velocity.Service.Transport;

/// <summary>
/// Represents a transport connection over a network socket. Implements the <see cref="ITransportConnection"/>
/// interface to provide access to a remote endpoint and a communication stream.
/// </summary>
public sealed class SocketTransportConnection : ITransportConnection
{
    private readonly Socket _socket;
    private readonly NetworkStream _stream;

    public SocketTransportConnection(Socket socket)
    {
        _socket = socket;
        _stream = new NetworkStream(socket, ownsSocket: true);

        Input = PipeReader.Create(_stream);
        Output = PipeWriter.Create(_stream);
    }

    public EndPoint? RemoteEndPoint => _socket.RemoteEndPoint;

    public PipeReader Input { get; }

    public PipeWriter Output { get; }

    public async ValueTask DisposeAsync()
    {
        await Output.CompleteAsync();
        await Input.CompleteAsync();
        await _stream.DisposeAsync();
    }
}