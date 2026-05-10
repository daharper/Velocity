namespace Velocity.Foundation.Enums;

/// <summary>
/// Declares the default text representation for an XMPP-related enumeration.
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public sealed class XmppEnumAttribute : Attribute
{
    public XmppEnumAttribute(string defaultText)
    {
        if (string.IsNullOrWhiteSpace(defaultText))
            throw new ArgumentException("Default text cannot be blank.", nameof(defaultText));

        DefaultText = defaultText;
    }

    public string DefaultText { get; }
}