using System.Net;
using System.Security;
using System.Xml;

namespace Velocity.Foundation.Xml;

/// <summary>
/// Provides utility methods for encoding and decoding XML element and attribute names,
/// as well as escaping and unescaping XML values to ensure proper handling of special characters.
/// </summary>
public static class BvXmlEncoder
{
    /// <summary>
    /// Encodes the specified name into a safe format for use in XML content, ensuring
    /// that special or invalid characters in the name are replaced with their respective
    /// valid XML representations.
    /// </summary>
    /// <param name="name">The name to encode, which may contain characters not allowed in XML names.</param>
    /// <returns>A string representing the XML-safe encoded version of the input name.</returns>
    public static string EncodeName(string name)
        => XmlConvert.EncodeName(name);

    /// <summary>
    /// Decodes the specified XML-safe encoded name back into its original format, restoring any
    /// special or invalid characters replaced during encoding.
    /// </summary>
    /// <param name="name">The XML-safe encoded name to decode, typically produced by an encoding method.</param>
    /// <returns>A string representing the original, unencoded version of the input name.</returns>
    public static string DecodeName(string name)
        => XmlConvert.DecodeName(name);

    /// <summary>
    /// Escapes the specified value by replacing special characters with their corresponding
    /// XML entity representations to ensure safe usage within XML content.
    /// </summary>
    /// <param name="value">
    /// The value to escape, which may include characters that need encoding to prevent XML parsing issues.
    /// </param>
    /// <returns>A string representing the escaped version of the input value, suitable for use within XML.</returns>
    public static string EscapeValue(string value)
        => SecurityElement.Escape(value);

    /// <summary>
    /// Unescapes the specified value by decoding XML or HTML entity representations
    /// back into their original characters, ensuring the value is restored to its
    /// intended human-readable form.
    /// </summary>
    /// <param name="value">The escaped value containing XML or HTML entities that need decoding.</param>
    /// <returns>A string representing the unescaped version of the input value.</returns>
    public static string UnescapeValue(string value)
        => WebUtility.HtmlDecode(value);
}