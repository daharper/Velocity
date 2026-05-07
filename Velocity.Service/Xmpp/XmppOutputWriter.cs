using System.Threading.Channels;

namespace Velocity.Service.Xmpp;

/// <summary>
/// Provides functionality to write XMPP stanzas to an output channel.
/// </summary>
public sealed class XmppOutputWriter : IXmppOutputWriter
{
    private ChannelWriter<string>? _writer;
    private XmppChannelMetrics _metrics;
    
    public XmppOutputWriter(XmppChannelMetrics metrics)
    {
        _metrics = metrics;
    }
    
    public bool IsAttached => _writer != null;
    
    /// <summary>
    /// Attaches a channel writer to the XmppOutputWriter, enabling it to write XMPP stanzas to the output channel.
    /// </summary>
    /// <param name="writer">The <see cref="ChannelWriter{T}"/> instance used for writing XMPP stanzas.</param>
    public void Attach(ChannelWriter<string> writer)
    {
        _writer = writer;
    }

    /// <summary>
    /// Writes an XML stanza asynchronously to the attached XMPP output channel.
    /// </summary>
    /// <param name="xml">The XML string representing the XMPP stanza to be written.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous write operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the output writer is not attached.</exception>
    public ValueTask WriteAsync(string xml, CancellationToken cancellationToken)
    {
        var writer = _writer ?? throw new InvalidOperationException("XMPP output writer is not attached.");

        _metrics.OutboundQueued();

        try
        {
            return writer.WriteAsync(xml, cancellationToken);
        }
        catch
        {
            _metrics.OutboundDequeued();
            throw;
        }
    }
}