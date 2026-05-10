namespace Velocity.Foundation.Enums;

using System.Reflection;

/// <summary>
/// Provides utility methods for handling enum values with XMPP-compliant text mappings.
/// </summary>
/// <example>
/// Conversion between enums and string representations is supported by methods such as:
/// <list type="bullet">
/// <item><description><see cref="AsEnum{TEnum}(string?)"/></description></item>
/// <item><description><see cref="AsEnumStrict{TEnum}(string)"/></description></item>
/// <item><description><see cref="AsText{TEnum}(TEnum)"/></description></item>
/// <item><description><see cref="TryGet{TEnum}(string?, out TEnum)"/></description></item>
/// </list>
/// These allow flexible mappings including strict and non-strict conversions.
/// </example>
public static class XmppEnum
{
    #region private
    
    // an enum cache for efficient lookups
    private static class Cache<TEnum> where TEnum : struct, Enum
    {
        private static readonly Dictionary<string, TEnum> TextToEnum;
        private static readonly Dictionary<TEnum, string> EnumToText;
        private static readonly TEnum DefaultValue;

        // create the cache from the enum type
        static Cache()
        {
            var enumType = typeof(TEnum);
            var enumAttribute = enumType.GetCustomAttribute<XmppEnumAttribute>();
            
            if (enumAttribute is null)
            {
                throw new InvalidOperationException(
                    $"Enum {enumType.FullName} is missing {nameof(XmppEnumAttribute)}.");
            }

            TextToEnum = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);
            EnumToText = new Dictionary<TEnum, string>();

            var defaultFound = false;

            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var textAttribute = field.GetCustomAttribute<XmppTextAttribute>();
                
                if (textAttribute is null)
                {
                    throw new InvalidOperationException(
                        $"Enum member {enumType.FullName}.{field.Name} is missing {nameof(XmppTextAttribute)}.");
                }

                var value = (TEnum)field.GetValue(null)!;
                var text = textAttribute.Text;

                TextToEnum.Add(text, value);
                EnumToText.Add(value, text);

                if (string.Equals(text, enumAttribute.DefaultText, StringComparison.OrdinalIgnoreCase))
                {
                    DefaultValue = value;
                    defaultFound = true;
                }
            }

            if (!defaultFound)
            {
                throw new InvalidOperationException(
                    $"Enum {enumType.FullName} does not contain an XMPP text value matching default '{enumAttribute.DefaultText}'.");
            }
        }

        // Converts a string representation of an enumeration value to its corresponding strongly-typed enum.
        public static TEnum AsEnum(string? text)
        {
            if (!string.IsNullOrWhiteSpace(text) && TextToEnum.TryGetValue(text, out var value))
            {
                return value;
            }

            return DefaultValue;
        }
        
        // Converts a string representation of an enumeration value to its corresponding strongly-typed enum.
        // Throws an exception if the input is invalid.
        public static TEnum AsEnumStrict(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("XMPP enum text cannot be blank.", nameof(text));

            if (TextToEnum.TryGetValue(text, out var value))
                return value;

            throw new ArgumentException(
                $"'{text}' is not a valid XMPP text value for enum {typeof(TEnum).FullName}.",
                nameof(text));
        }

        
        // Attempts to retrieve the corresponding strongly-typed enum value for the given string representation.
        public static bool TryGet(string? text, out TEnum value)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                value = default;
                return false;
            }

            return TextToEnum.TryGetValue(text, out value);
        }
        
        // Converts a strongly-typed enum value to its corresponding string representation.
        public static string AsText(TEnum value)
        {
            if (EnumToText.TryGetValue(value, out var text)) return text;

            throw new ArgumentException(
                $"Enum value '{value}' is not mapped for {typeof(TEnum).FullName}.",
                nameof(value));
        }
    }

    #endregion
    
    /// <summary>
    /// Converts a string representation of a value to its corresponding strongly-typed enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to which the string will be converted.</typeparam>
    /// <param name="text">
    /// The string value to convert to the corresponding enumeration value. Can be null or whitespace.
    /// </param>
    /// <returns>
    /// The corresponding enumeration value if the string is successfully matched; otherwise,
    /// the default value of the specified enumeration type.
    /// </returns>
    public static TEnum AsEnum<TEnum>(string? text) where TEnum : struct, Enum
        => Cache<TEnum>.AsEnum(text);

    /// <summary>
    /// Converts a string representation of an enumeration value to its corresponding strongly-typed
    /// enum using strict matching.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to convert to.</typeparam>
    /// <param name="text">
    /// The string value to convert to the corresponding enum. Must not be null, empty, or whitespace.
    /// </param>
    /// <returns>
    /// The enum value matching the specified string.
    /// If the string does not match any defined enum values, an exception is thrown.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// Thrown when the provided string is null, empty, whitespace, or does not correspond
    /// to any defined value of the enum.
    /// </exception>
    public static TEnum AsEnumStrict<TEnum>(string text) where TEnum : struct, Enum
        => Cache<TEnum>.AsEnumStrict(text);

    /// <summary>
    /// Attempts to parse the specified string value into its corresponding strongly-typed enum.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to parse. Must be a struct and derive from Enum.</typeparam>
    /// <param name="text">The string value to parse. Can be null or whitespace.</param>
    /// <param name="value">
    /// When this method returns, contains the enum value corresponding to the specified string
    /// if the parsing succeeds, or the default value of <typeparamref name="TEnum"/> if it fails.
    /// </param>
    /// <returns>
    /// true if the parsing succeeds and a matching enum value is found; otherwise, false.
    /// </returns>
    public static bool TryGet<TEnum>(string? text, out TEnum value) where TEnum : struct, Enum
        => Cache<TEnum>.TryGet(text, out value);

    /// <summary>
    /// Converts an enum value to its corresponding XMPP-compliant text representation.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to convert. Must be a struct and derive from Enum.</typeparam>
    /// <param name="value">The enum value to convert to its string representation.</param>
    /// <returns>
    /// The XMPP-compliant text representation of the specified enum value.
    /// If a mapping does not exist for the given value, an exception is thrown.
    /// </returns>
    public static string AsText<TEnum>(TEnum value) where TEnum : struct, Enum
        => Cache<TEnum>.AsText(value);
}