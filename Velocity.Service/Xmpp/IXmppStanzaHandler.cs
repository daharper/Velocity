namespace Velocity.Service.Xmpp;

public interface IXmppStanzaHandler
{
    ValueTask HandleAsync(string stanza, CancellationToken cancellationToken);
}