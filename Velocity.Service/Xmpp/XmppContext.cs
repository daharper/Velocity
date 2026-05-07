using Velocity.Service.Identity;

namespace Velocity.Service.Xmpp;

/// <summary>
/// Represents the context for processing an XMPP stanza within the middleware pipeline.
/// </summary>
public sealed class XmppContext
{
    /// <summary>
    /// Gets the raw XML string representation of the XMPP stanza 
    /// currently being processed within the context of the pipeline.
    /// </summary>
    /// <remarks>
    /// This will be replaced with an object once the stanza parsing is in place.
    /// </remarks>
    public required string Stanza { get; init; }

    /// <summary>
    /// Gets or sets the endpoint that should handle the current XMPP stanza within the middleware pipeline.
    /// </summary>
    public XmppEndpoint? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the authorization profile associated with the current XMPP context,
    /// which provides user-specific authorization information, including JID, primary role,
    /// and group roles.
    /// </summary>
    public AuthorizationProfile? Authorization { get; set; }
    
    /// <summary>
    /// Gets the output writer responsible for sending XMPP stanzas to the connection's socket.
    /// </summary>
    public required IXmppOutputWriter Output { get; init; }

    /// <summary>
    /// Provides access to the application's service provider, allowing resolution of dependencies
    /// and services within the XMPP context.
    /// </summary>
    public required IServiceProvider Services { get; init; }

    /// <summary>
    /// Gets the <see cref="CancellationToken"/> used to monitor
    /// for cancellation requests within the current XMPP context.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }
}