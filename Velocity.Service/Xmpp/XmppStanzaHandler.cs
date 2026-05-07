using Microsoft.Extensions.Logging;

namespace Velocity.Service.Xmpp;

public sealed class XmppStanzaHandler : IXmppStanzaHandler
{
    private readonly IXmppOutputWriter _outputWriter;
    private readonly ILogger<XmppStanzaHandler> _logger;

    public XmppStanzaHandler(
        IXmppOutputWriter outputWriter,
        ILogger<XmppStanzaHandler> logger)
    {
        _outputWriter = outputWriter;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        string stanza,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Dispatching stanza with {Length} character(s)",
            stanza.Length);

        // temporary test response later if needed
        await ValueTask.CompletedTask;
    }
}