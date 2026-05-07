using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Velocity.Service.Networking;
using Velocity.Service.Transport;

namespace Velocity.Service.Xmpp;

public sealed record XmppInputFrame(byte[] Buffer);

public sealed class XmppConnectionHandler : IXmppConnectionHandler
{
    private readonly IXmppStreamParser _parser;
    private readonly ILogger<XmppConnectionHandler> _logger;
    private readonly TcpClientOptions _clientOptions;
    
    public XmppConnectionHandler(
        IXmppStreamParser parser,
        IOptions<TcpClientOptions> clientOptions,
        ILogger<XmppConnectionHandler> logger)
    {
        _parser = parser;
        _clientOptions = clientOptions.Value;
        _logger = logger;
    }

    public async Task HandleAsync(
        ITransportConnection connection,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "XMPP connection established to {RemoteEndPoint}",
            connection.RemoteEndPoint);

        await OpenStreamAsync(connection.Output, cancellationToken);
        
        await RunReadLoopAsync(connection.Input, cancellationToken);
    }

    private async Task RunReadLoopAsync(
        PipeReader input,
        CancellationToken cancellationToken)
    {
        Decoder decoder = Encoding.UTF8.GetDecoder();

        char[] chars = ArrayPool<char>.Shared.Rent(4096);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ReadResult result = await input.ReadAsync(cancellationToken);

                ReadOnlySequence<byte> buffer = result.Buffer;

                foreach (ReadOnlyMemory<byte> segment in buffer)
                {
                    DecodeAndParse(
                        decoder,
                        segment.Span,
                        chars,
                        flush: result.IsCompleted,
                        cancellationToken);
                }

                input.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
        }
    }

    private void DecodeAndParse(
        Decoder decoder,
        ReadOnlySpan<byte> bytes,
        char[] chars,
        bool flush,
        CancellationToken cancellationToken)
    {
        while (!bytes.IsEmpty)
        {
            decoder.Convert(
                bytes,
                chars,
                flush,
                out int bytesUsed,
                out int charsUsed,
                out bool completed);

            if (charsUsed > 0)
            {
                _parser.Parse(
                    chars.AsSpan(0, charsUsed),
                    cancellationToken);
            }

            bytes = bytes[bytesUsed..];

            if (completed)
            {
                break;
            }
        }
    }
    
    private async Task OpenStreamAsync(
        PipeWriter output,
        CancellationToken cancellationToken)
    {
        var xml = $"""<stream:stream xmlns:stream='http://etherx.jabber.org/streams' xml:lang='en' version='1.0' xmlns='jabber:component:accept' to='{_clientOptions.ComponentName}'>""";

        await output.WriteAsync(
            Encoding.UTF8.GetBytes(xml),
            cancellationToken);

        await output.FlushAsync(cancellationToken);
    }
}