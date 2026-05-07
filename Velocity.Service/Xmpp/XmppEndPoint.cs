using System.Reflection;

namespace Velocity.Service.Xmpp;

/// <summary>
/// Represents an XMPP processing endpoint that binds an action method to an XMPP stanza, providing all necessary
/// information for invocation, parameter resolution, and middleware execution.
/// </summary>
public sealed class XmppEndpoint
{
    public required Type ControllerType { get; init; }

    public required MethodInfo Action { get; init; }

    public required IReadOnlyList<XmppParameterBinding> Parameters { get; init; }

    public required Func<XmppContext, ValueTask> RequestDelegate { get; init; }
}