namespace Velocity.Foundation.Enums;

public static class XmppEnumExtensions
{
    public static string AsXmppText<TEnum>(this TEnum value) where TEnum : struct, Enum
        => XmppEnum.AsText(value);
 
    public static TEnum AsXmppEnum<TEnum>(this string? text) where TEnum : struct, Enum
        => XmppEnum.AsEnum<TEnum>(text);

    public static TEnum AsStrictXmppEnum<TEnum>(this string text) where TEnum : struct, Enum
        => XmppEnum.AsEnumStrict<TEnum>(text);

    public static bool TryGetXmppEnum<TEnum>(this string? text, out TEnum value) where TEnum : struct, Enum
        => XmppEnum.TryGet(text, out value);
}