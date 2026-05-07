namespace Velocity.Service.Xmpp;

public sealed class XmppResponseWriter : IXmppResponseWriter
{
    public async ValueTask WriteResponseAsync(XmppContext context, object? response)
    {
        // todo: null response might also need a reply 

        if (response is string xml)
        {
            await context.Output.WriteAsync(xml, context.CancellationToken);
        }

        // todo: object → XML payload → reply stanza
    }
}