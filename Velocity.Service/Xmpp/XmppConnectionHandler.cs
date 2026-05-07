using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Velocity.Service.Networking;
using Velocity.Service.Transport;

namespace Velocity.Service.Xmpp;

/// <summary>
/// Handles the establishment and management of XMPP connections.
/// </summary>
public sealed class XmppConnectionHandler : IXmppConnectionHandler
{
    #region private
    
    private readonly IXmppStreamParser _parser;
    private readonly XmppOutputWriter _outputWriter;
    private readonly TcpClientOptions _clientOptions;
    private readonly IXmppPipeline _pipeline;
    private readonly IServiceProvider _services;
    private readonly ILogger<XmppConnectionHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="XmppConnectionHandler"/> class.
    /// </summary>
    /// <param name="parser">The parser responsible for interpreting XMPP stream data into stanzas. </param>
    /// <param name="outputWriter">The writer used to send XMPP data to the remote party.</param>
    /// <param name="pipeline">The pipeline for processing XMPP routing and events.</param>
    /// <param name="services">The service provider for resolving application-level dependencies.</param>
    /// <param name="clientOptions">The configuration options for the TCP client connection.</param>
    /// <param name="logger">The logger instance for event and error reporting related to the connection handler.</param>
    public XmppConnectionHandler(
        IXmppStreamParser parser,
        XmppOutputWriter outputWriter,
        IXmppPipeline pipeline,
        IServiceProvider services,
        IOptions<TcpClientOptions> clientOptions,
        ILogger<XmppConnectionHandler> logger)
    {
        _parser = parser;
        _outputWriter = outputWriter;
        _pipeline = pipeline;
        _services = services;
        _clientOptions = clientOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Handles the XMPP connection by processing incoming and outgoing data streams asynchronously
    /// and dispatching to the parser.
    /// </summary>
    /// <param name="connection">
    /// The transport connection representing the underlying network communication channel.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to propagate notifications of cancellation requests.
    /// </param>
    /// <returns>A task representing the asynchronous operation of handling the connection.</returns>
    public async Task HandleAsync(ITransportConnection connection, CancellationToken cancellationToken)
    {
        var inbound = Channel.CreateBounded<string>(
            new BoundedChannelOptions(_clientOptions.InboundChannelCapacity)
            {
                SingleWriter = true,
                SingleReader = false,
                FullMode = BoundedChannelFullMode.Wait
            });

        var outbound = Channel.CreateBounded<string>(
            new BoundedChannelOptions(_clientOptions.OutboundChannelCapacity)
            {
                SingleWriter = false,
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait
            });

        _outputWriter.Attach(outbound.Writer);

        _logger.LogInformation("XMPP connection established to {RemoteEndPoint}", connection.RemoteEndPoint);

        var readLoop = RunReadLoopAsync(connection.Input, inbound.Writer, cancellationToken);
        var dispatchLoop = RunDispatchLoopAsync(inbound.Reader, cancellationToken);
        var writeLoop = RunWriteLoopAsync(outbound.Reader, connection.Output, cancellationToken);

        await _outputWriter.WriteAsync(
            CreateOpeningStreamXml(),
            cancellationToken);

        try
        {
            await Task.WhenAll(readLoop, dispatchLoop);
        }
        finally
        {
            inbound.Writer.TryComplete();
            outbound.Writer.TryComplete();
        }

        await writeLoop;
    }

    /// <summary>
    /// Continuously reads data from the provided <see cref="PipeReader"/> and publishes parsed XMPP stanzas to the
    /// specified <see cref="ChannelWriter{T}"/> until cancellation is requested or the input stream is completed.
    /// </summary>
    /// <param name="input">The pipe reader for reading data from the underlying transport connection.</param>
    /// <param name="inboundWriter">
    /// The channel writer for publishing parsed XMPP stanzas from the incoming data stream.
    /// </param>
    /// <param name="cancellationToken">The token used to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous read loop operation.</returns>
    private async Task RunReadLoopAsync(
        PipeReader input,
        ChannelWriter<string> inboundWriter,
        CancellationToken cancellationToken)
    {
        var decoder = Encoding.UTF8.GetDecoder();
        var chars = ArrayPool<char>.Shared.Rent(_clientOptions.ReceiveBufferSize);

        List<string> stanzas = new(capacity: 32);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await input.ReadAsync(cancellationToken);
                var buffer = result.Buffer;

                foreach (var segment in buffer)
                {
                    await DecodeParseAndPublishAsync(
                        decoder,
                        segment,
                        chars,
                        stanzas,
                        inboundWriter,
                        flush: result.IsCompleted,
                        cancellationToken);
                }

                input.AdvanceTo(buffer.End);

                if (result.IsCompleted) break;
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
            inboundWriter.TryComplete();
        }
    }

    /// <summary>
    /// Decodes byte data into characters, parses the resulting character data into XMPP stanzas,
    /// and publishes the parsed stanzas to an inbound channel.
    /// </summary>
    /// <param name="decoder">The decoder used to convert the byte data into characters.</param>
    /// <param name="bytes">The input buffer containing the bytes to be decoded.</param>
    /// <param name="chars">The character array used as a buffer for decoding operations.</param>
    /// <param name="stanzas">The list used to store parsed XMPP stanzas during processing.</param>
    /// <param name="inboundWriter">The channel writer where parsed stanzas are published for further processing.</param>
    /// <param name="flush">A value indicating whether to flush the decoder after processing the current data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    private async ValueTask DecodeParseAndPublishAsync(
        Decoder decoder,
        ReadOnlyMemory<byte> bytes,
        char[] chars,
        List<string> stanzas,
        ChannelWriter<string> inboundWriter,
        bool flush,
        CancellationToken cancellationToken)
    {
        var remaining = bytes;

        while (!remaining.IsEmpty)
        {
            decoder.Convert(remaining.Span, chars, flush, out int bytesUsed, out int charsUsed, out bool completed);

            if (charsUsed > 0)
            {
                _parser.Parse(chars.AsSpan(0, charsUsed), stanzas, cancellationToken);
                
                foreach (var stanza in stanzas)
                {
                    await inboundWriter.WriteAsync(stanza, cancellationToken);
                }

                stanzas.Clear();
            }

            remaining = remaining[bytesUsed..];

            if (completed) break;
        }
    }

    /// <summary>
    /// Processes incoming XMPP stanzas in a continuous loop,
    /// dispatching them through the configured pipeline for handling.
    /// </summary>
    /// <param name="reader">The channel reader that provides incoming XMPP stanzas for processing.</param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the loop to terminate gracefully.
    /// </param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation of the dispatch loop.</returns>
    private async Task RunDispatchLoopAsync(ChannelReader<string> reader, CancellationToken cancellationToken)
    {
        await foreach (var stanza in reader.ReadAllAsync(cancellationToken))
        {
            var context = new XmppContext
            {
                Stanza = stanza,
                Output = _outputWriter,
                Services = _services,
                CancellationToken = cancellationToken
            };

            await _pipeline.ExecuteAsync(context);
        }
    }

    /// <summary>
    /// Continuously reads XML data from the specified channel reader and writes it to the output pipe in UTF-8 format.
    /// </summary>
    /// <param name="reader">The channel reader from whom XML data is read.</param>
    /// <param name="output">The pipe writer to whom the XML data is written.</param>
    /// <param name="cancellationToken">
    /// A token to observe while performing asynchronous operations to cancel the loop if needed.
    /// </param>
    /// <returns>A task that represents the asynchronous operation of writing data to the output pipe.</returns>
    private async Task RunWriteLoopAsync(
        ChannelReader<string> reader,
        PipeWriter output,
        CancellationToken cancellationToken)
    {
        await foreach (var xml in reader.ReadAllAsync(cancellationToken))
        {
            await WriteUtf8Async(output, xml, cancellationToken);
            await output.FlushAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Writes the specified string value as UTF-8 encoded bytes to the provided <see cref="PipeWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="PipeWriter"/> to which the UTF-8 encoded bytes are written.</param>
    /// <param name="value">The string value to encode and write to the writer.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous write operation.</returns>
    private async ValueTask WriteUtf8Async(
        PipeWriter writer,
        string value,
        CancellationToken cancellationToken)
    {
        var encoder = Encoding.UTF8.GetEncoder();
        var offset = 0;

        while (offset < value.Length)
        {
            var memory = writer.GetMemory(_clientOptions.SendBufferSize);

            encoder.Convert(
                value.AsSpan(offset),
                memory.Span,
                flush: true,
                out int charsUsed,
                out int bytesUsed,
                out bool completed);

            writer.Advance(bytesUsed);

            offset += charsUsed;

            if (completed) break;

            var result = await writer.FlushAsync(cancellationToken);
            if (result.IsCompleted || result.IsCanceled) break;
        }
    }

    // temporary stream ingestion to begin data transfer to build out the plumbing
    private string CreateOpeningStreamXml()
    {
        return $"""<stream:stream xmlns="jabber:component:accept" xmlns:stream="http://etherx.jabber.org/streams" to="{_clientOptions.ComponentName}">""";
    }
}