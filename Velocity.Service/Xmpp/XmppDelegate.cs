namespace Velocity.Service.Xmpp;

/// <summary>
/// Represents a delegate used to process an XMPP context within the middleware pipeline.
/// </summary>
/// <param name="context">The <see cref="XmppContext"/> representing the current XMPP stanza and services.</param>
/// <returns>A task that represents the asynchronous execution of the delegate.</returns>
public delegate ValueTask XmppDelegate(XmppContext context);