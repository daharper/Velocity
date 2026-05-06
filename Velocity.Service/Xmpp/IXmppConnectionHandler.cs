using Velocity.Service.Transport;

namespace Velocity.Service.Xmpp;

public interface IXmppConnectionHandler
{
    Task HandleAsync(
        ITransportConnection connection,
        CancellationToken cancellationToken);
}