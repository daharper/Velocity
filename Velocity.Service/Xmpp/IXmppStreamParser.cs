namespace Velocity.Service.Xmpp;

/// <summary>
/// Provides methods for parsing XML data from an XMPP stream into stanzas.
/// </summary>
public interface IXmppStreamParser
{
    /// <summary>
    /// Parses a span of characters containing XML data into individual XMPP stanzas and adds them to the provided collection.
    /// </summary>
    /// <param name="chars">The span of characters containing the XML data to be parsed.</param>
    /// <param name="stanzas">The collection to which parsed stanzas will be added.</param>
    /// <param name="cancellationToken">Token used to monitor for cancellation requests.</param>
    void Parse(ReadOnlySpan<char> chars, ICollection<string> stanzas, CancellationToken cancellationToken);
}