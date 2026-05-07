using System.Reflection;

namespace Velocity.Service.Xmpp;

/// <summary>
/// Represents a binding between a method parameter and an XMPP stanza element, enabling the resolution
/// of parameter values during the invocation of an XMPP endpoint action.
/// </summary>
public sealed class XmppParameterBinding
{
    public required ParameterInfo Parameter { get; init; }
    
    public required string SourceElementName { get; init; }
}