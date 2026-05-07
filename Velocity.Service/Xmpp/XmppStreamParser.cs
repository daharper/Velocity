using System.Text;

namespace Velocity.Service.Xmpp;

using Microsoft.Extensions.Logging;

/// <summary>
/// Parses XMPP stream data and processes it into stanzas for further handling.
/// </summary>
public sealed class XmppStreamParser : IXmppStreamParser
{
    #region private
    
    private readonly ILogger<XmppStreamParser> _logger;
    private readonly StringBuilder _buffer = new();

    #endregion
    
    /// <summary>
    /// Initializes a new instance of the <see cref="XmppStreamParser"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public XmppStreamParser(ILogger<XmppStreamParser> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Parses the provided XMPP stream data and adds processed stanzas to the given collection.
    /// </summary>
    /// <param name="chars">The XMPP stream data represented as a read-only span of characters.</param>
    /// <param name="stanzas">The collection to which the parsed stanzas should be added.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    public void Parse(
        ReadOnlySpan<char> chars,
        ICollection<string> stanzas,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (!chars.IsEmpty)
        {
            stanzas.Add(chars.ToString());
        }
        //
        // foreach (char c in chars)
        // {
        //     _buffer.Append(c);
        //
        //     if (c == '\n')
        //     {
        //         stanzas.Add(_buffer.ToString());
        //         _buffer.Clear();
        //     }
        // }
    }
    
    public void Reset()
    {
        _buffer.Clear();
    }
}