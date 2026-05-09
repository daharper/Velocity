using System.Xml;

namespace Velocity.Foundation.Xml;

/// <summary>
/// Provides utility methods to validate XML names and values based on the XML specification.
/// </summary>
public static class BvXmlValidator
{
    private const int DefaultMaxXmlNameLength = 1024;

    /// <summary>
    /// Validates whether the specified string is a valid XML name.
    /// </summary>
    /// <param name="name">The string to validate as an XML name. Can be null or empty.</param>
    /// <param name="maxLength">The maximum allowed length for the XML name. Defaults to a predefined value if not specified.</param>
    /// <returns>
    /// Returns true if the specified string is a valid XML name and does not exceed the specified length.
    /// Returns false if the input is null, empty, fails XML naming rules, or exceeds the maximum length.
    /// </returns>
    public static bool IsValidName(string? name, int maxLength = DefaultMaxXmlNameLength)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (maxLength > 0 && name.Length > maxLength) return false;

        try
        {
            XmlConvert.VerifyName(name);
            return true;
        }
        catch (XmlException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates whether the specified string is a valid XML value.
    /// </summary>
    /// <param name="value">The string to validate as an XML value. Can be null.</param>
    /// <returns>
    /// Returns true if the string contains only characters that are valid according to the XML specification.
    /// Returns false if the string is null or contains invalid XML characters.
    /// </returns>
    public static bool IsValidValue(string? value)
        => value is not null && value.EnumerateRunes().All(rune => IsValidChar(rune.Value));

    /// <summary>
    /// Determines whether the specified Unicode character value is a valid character
    /// according to the XML specification.
    /// </summary>
    /// <param name="value">The Unicode character value to validate.</param>
    /// <returns>
    /// Returns true if the specified character value is considered valid according to the XML specification.
    /// Returns false if the character value falls outside the ranges permitted by the specification.
    /// </returns>
    private static bool IsValidChar(int value)
    {
        return
            value is 0x9 or 0xA ||
            value == 0xD ||
            value is >= 0x20 and <= 0xD7FF ||
            value is >= 0xE000 and <= 0xFFFD ||
            value is >= 0x10000 and <= 0x10FFFF;
    }
}