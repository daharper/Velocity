namespace Velocity.Foundation.Enums;

/// <summary>
/// Declares the text representation for an XMPP-related enumeration field.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class XmppTextAttribute : Attribute
{
    public XmppTextAttribute(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be blank.", nameof(text));

        Text = text;
    }

    public string Text { get; }
}