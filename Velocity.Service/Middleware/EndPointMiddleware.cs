using Microsoft.Extensions.Logging;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Middleware;

/// <summary>
/// Middleware placeholder for building out the pipeline
/// </summary>
public sealed class EndPointMiddleware(ILogger<RouterMiddleware> logger) : IXmppMiddleware
{
    public async ValueTask InvokeAsync(XmppContext context, XmppDelegate next)
    {
        logger.LogInformation("EndPoint: received stanza with {Length} character(s)", context.Stanza.Length);

        await next(context);
    }
}