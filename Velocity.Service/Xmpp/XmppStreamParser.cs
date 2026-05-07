namespace Velocity.Service.Xmpp;

using Microsoft.Extensions.Logging;

public sealed class XmppStreamParser : IXmppStreamParser
{
    private readonly ILogger<XmppStreamParser> _logger;
    private long _totalCharacters;

    public XmppStreamParser(
        ILogger<XmppStreamParser> logger)
    {
        _logger = logger;
    }

    public long TotalCharacters => _totalCharacters;

    public void Parse(ReadOnlySpan<char> chars, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (chars.IsEmpty) return;

        _totalCharacters += chars.Length;

        _logger.LogDebug(
            "Parsed {CharacterCount} character(s). Total={TotalCharacters}",
            chars.Length,
            _totalCharacters);
    }
}