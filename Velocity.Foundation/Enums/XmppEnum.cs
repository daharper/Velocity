namespace Velocity.Foundation.Enums;

using System.Reflection;

public static class XmppEnum
{
    private static class Cache<TEnum> where TEnum : struct, Enum
    {
        private static readonly Dictionary<string, TEnum> TextToEnum;
        private static readonly Dictionary<TEnum, string> EnumToText;
        private static readonly TEnum DefaultValue;

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

                if (string.Equals(
                        text,
                        enumAttribute.DefaultText,
                        StringComparison.OrdinalIgnoreCase))
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

        public static TEnum AsEnum(string? text)
        {
            if (!string.IsNullOrWhiteSpace(text) && TextToEnum.TryGetValue(text, out var value))
            {
                return value;
            }

            return DefaultValue;
        }

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

        public static bool TryGet(string? text, out TEnum value)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                value = default;
                return false;
            }

            return TextToEnum.TryGetValue(text, out value);
        }

        public static string AsText(TEnum value)
        {
            if (EnumToText.TryGetValue(value, out var text)) return text;

            throw new ArgumentException(
                $"Enum value '{value}' is not mapped for {typeof(TEnum).FullName}.",
                nameof(value));
        }
    }
    
    public static TEnum AsEnum<TEnum>(string? text) where TEnum : struct, Enum
        => Cache<TEnum>.AsEnum(text);

    public static TEnum AsEnumStrict<TEnum>(string text) where TEnum : struct, Enum
        => Cache<TEnum>.AsEnumStrict(text);
 
    public static bool TryGet<TEnum>(string? text, out TEnum value) where TEnum : struct, Enum
        => Cache<TEnum>.TryGet(text, out value);

    public static string AsText<TEnum>(TEnum value) where TEnum : struct, Enum
        => Cache<TEnum>.AsText(value);
}