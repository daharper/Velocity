using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Velocity.Service.Networking;

namespace Velocity.Service.Transport;

/// <summary>
/// Represents a transport connection over a socket providing read and write
/// data streams through <see cref="PipeReader"/> and <see cref="PipeWriter"/>
/// for asynchronous communication.
/// </summary>
public sealed class SocketTransportConnection : ITransportConnection
{
    #region private
    
    private readonly Socket _socket;
    private readonly Pipe _input = new();
    private readonly Pipe _output = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly TcpClientOptions _clientOptions;
    
    private readonly Task _receiveLoop;
    private readonly Task _sendLoop;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SocketTransportConnection"/> class.
    /// </summary>
    /// <param name="socket">The underlying socket for the transport connection.</param>
    /// <param name="clientOptions">Options for configuring the client behavior.</param>
    public SocketTransportConnection(Socket socket, TcpClientOptions clientOptions)
    {
        _socket = socket;
        _clientOptions = clientOptions;
        _receiveLoop = RunReceiveLoopAsync(_cts.Token);
        _sendLoop = RunSendLoopAsync(_cts.Token);
    }

    /// <summary>
    /// Gets the remote endpoint associated with the connection; represents the network address and port number
    /// </summary>
    public EndPoint? RemoteEndPoint => _socket.RemoteEndPoint;

    /// <summary>
    /// Gets the reader for the input pipe, which provides access to the incoming data stream
    /// associated with the transport connection.
    /// </summary>
    public PipeReader Input => _input.Reader;

    /// <summary>
    /// Provides access to the writable end of the pipeline for sending data
    /// through the transport connection.
    /// </summary>
    public PipeWriter Output => _output.Writer;

    /// <summary>
    /// Executes the receive loop for handling incoming data asynchronously through the socket.
    /// Data is read into the input pipe and made available for further processing.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the receive loop operation.</param>
    /// <return>A task that represents the asynchronous operation of the receive loop.</return>
    private async Task RunReceiveLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var memory = _input.Writer.GetMemory(_clientOptions.ReceiveBufferSize);

                var bytesRead = await _socket.ReceiveAsync(memory, SocketFlags.None, cancellationToken);
                if (bytesRead == 0) break;

                _input.Writer.Advance(bytesRead);

                var flush = await _input.Writer.FlushAsync(cancellationToken);
                if (flush.IsCompleted || flush.IsCanceled) break;
            }
        }
        finally
        {
            await _input.Writer.CompleteAsync();
        }
    }

    /// <summary>
    /// Executes the send loop for transmitting data from the output pipe to the underlying socket asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the send loop operation.</param>
    /// <returns>A task that represents the asynchronous operation of the send loop.</returns>
    private async Task RunSendLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _output.Reader.ReadAsync(cancellationToken);
                var buffer = result.Buffer;

                foreach (var segment in buffer)
                {
                    if (!segment.IsEmpty)
                    {
                        await _socket.SendAsync(segment, SocketFlags.None, cancellationToken);
                    }
                }

                _output.Reader.AdvanceTo(buffer.End);

                if (result.IsCompleted) break;
            }
        }
        finally
        {
            await _output.Reader.CompleteAsync();
        }
    }

    /// <summary>
    /// Asynchronously releases the resources used by the <see cref="SocketTransportConnection"/> instance,
    /// including canceling operations, completing pipes, and disposing of the underlying socket.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous dispose operation.
    /// </returns>
    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();

        await _output.Writer.CompleteAsync();
        await _input.Reader.CompleteAsync();

        try
        {
            await Task.WhenAll(_receiveLoop, _sendLoop);
        }
        catch (OperationCanceledException)
        {
        }

        _socket.Dispose();
        _cts.Dispose();
    }
}