namespace Velocity.Service.Xmpp;

public interface IXmppResponseWriter
{
    ValueTask WriteResponseAsync(XmppContext context, object? response);
}