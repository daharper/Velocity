using Microsoft.Extensions.Logging;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Middleware;

/// <summary>
/// Middleware component responsible for handling exceptions that occur during the execution of the XMPP
/// processing pipeline. Captures unhandled exceptions (excluding <see cref="OperationCanceledException"/>),
/// logs them, and optionally sends an error response stanza back to the client.
/// </summary>
public sealed class ExceptionHandlingMiddleware(
    IXmppResponseWriter responseWriter, 
    ILogger<ExceptionHandlingMiddleware> logger) : IXmppMiddleware
{
    private readonly IXmppResponseWriter _responseWriter = responseWriter;

    public async ValueTask InvokeAsync(XmppContext context, XmppDelegate next)
    {
        try
        {
            logger.LogInformation("ExceptionHandler: received stanza with {Length} character(s)", context.Stanza.Length);
            
            await next(context);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unhandled exception while processing XMPP stanza.");

            // todo - write error stanza reply
            // await _responseWriter.WriteResponseAsync(
            //     context,
            //     new XmppErrorResult
            //     {
            //         Condition = "internal-server-error",
            //         Message = "The request failed."
            //     });
        }
    }
}