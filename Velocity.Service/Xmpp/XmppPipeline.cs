namespace Velocity.Service.Xmpp;

/// <summary>
/// Represents an implementation of the IXmppPipeline interface, providing
/// a framework for constructing and executing middleware pipelines for
/// processing XMPP operations.
/// </summary>
public sealed class XmppPipeline : IXmppPipeline
{
    private readonly XmppDelegate _pipeline;

    /// <summary>
    /// Represents an implementation of the IXmppPipeline interface, providing a middleware pipeline
    /// for processing XMPP operations. This pipeline allows middleware components to process XMPP contexts
    /// in a sequential and reusable manner.
    /// </summary>
    public XmppPipeline(IEnumerable<IXmppMiddleware> middleware)
    {
        XmppDelegate terminal = static _ => ValueTask.CompletedTask;

        _pipeline = middleware
            .Reverse()
            .Aggregate(
                terminal,
                static (next, component) =>
                    context => component.InvokeAsync(context, next));
    }

    /// <summary>
    /// Executes the pipeline processing for the specified XMPP context.
    /// </summary>
    /// <param name="context">The XMPP context containing the stanza, output writer, and services</param>
    /// <returns>A value task representing the asynchronous execution of the pipeline.</returns>
    public ValueTask ExecuteAsync(XmppContext context) => _pipeline(context);
}