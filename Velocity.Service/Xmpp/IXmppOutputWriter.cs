namespace Velocity.Service.Xmpp;

/// <summary>
/// Defines the contract for writing XMPP Stanzas to the socket.
/// </summary>
public interface IXmppOutputWriter
{
    bool IsAttached { get; }
    
    /// <summary>
    /// Writes an XML stanza asynchronously to the attached XMPP output channel.
    /// </summary>
    /// <param name="xml">The XML string representing the XMPP stanza to be written.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation of writing the XML stanza.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the output writer is not attached.</exception>
    ValueTask WriteAsync(string xml, CancellationToken cancellationToken);
}