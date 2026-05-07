using Microsoft.Extensions.Logging;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Middleware;

/// <summary>
/// Middleware placeholder for building out the pipeline
/// </summary>
public sealed class EndpointMiddleware(ILogger<EndpointMiddleware> logger) : IXmppMiddleware
{
    public async ValueTask InvokeAsync(XmppContext context, XmppDelegate next)
    {
        logger.LogInformation("EndPoint: received stanza with {Length} character(s)", context.Stanza.Length);

        // object? response = await endpoint.InvokeAsync(context);
        
        // await _responseWriter.WriteResponseAsync(context, response);
        
        await next(context);
    }
}