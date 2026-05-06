using System.Net;
using System.Net.Sockets;

namespace Velocity.Service.Transport;

/// <summary>
/// Represents a transport connection over a network socket. Implements the <see cref="ITransportConnection"/>
/// interface to provide access to a remote endpoint and a communication stream.
/// </summary>
public sealed class SocketTransportConnection(Socket socket) : ITransportConnection
{
    private readonly NetworkStream _stream = new(socket, ownsSocket: true);

    public EndPoint? RemoteEndPoint => socket.RemoteEndPoint;

    public Stream Stream => _stream;

    public async ValueTask DisposeAsync()
        => await _stream.DisposeAsync();
}