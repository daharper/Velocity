namespace Velocity.Service.Xmpp;

/// <summary>
/// Represents a middleware component in the XMPP processing pipeline.
/// </summary>
public interface IXmppMiddleware
{
    /// <summary>
    /// Processes the given <see cref="XmppContext"/> using the middleware component and invokes the next delegate in the pipeline.
    /// </summary>
    /// <param name="context">An <see cref="XmppContext"/> representing the current context</param>
    /// <param name="next">The delegate representing the next middleware to be executed in the pipeline.</param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous execution of the middleware.
    /// </returns>
    ValueTask InvokeAsync(XmppContext context, XmppDelegate next);
}