namespace Velocity.Service.Xmpp;

/// <summary>
/// Defines a pipeline for processing XMPP protocol operations.
/// </summary>
public interface IXmppPipeline
{
    /// <summary>
    /// Executes the middleware pipeline with the specified XMPP context.
    /// </summary>
    /// <param name="context">
    /// An instance of <see cref="XmppContext"/> that contains information about
    /// the XMPP stanza, output writer, service provider, and cancellation token
    /// for processing within the pipeline.
    /// </param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous execution of the middleware pipeline.</returns>
    ValueTask ExecuteAsync(XmppContext context);
}