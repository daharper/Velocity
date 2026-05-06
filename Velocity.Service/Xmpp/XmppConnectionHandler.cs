using Microsoft.Extensions.Logging;
using Velocity.Service.Transport;

namespace Velocity.Service.Xmpp;
    
public sealed class XmppConnectionHandler : IXmppConnectionHandler
{
    private readonly ILogger<XmppConnectionHandler> _logger;

    public XmppConnectionHandler(ILogger<XmppConnectionHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(ITransportConnection connection, CancellationToken cancellationToken)
    {
        _logger.LogInformation("XMPP connection established to {RemoteEndPoint}", connection.RemoteEndPoint);

        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }
}