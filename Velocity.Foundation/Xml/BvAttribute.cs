using System.Globalization;
using System.Xml;

namespace Velocity.Foundation.Xml;

/// <summary>
/// Represents an XML attribute with a name and value, providing functionality to validate,
/// convert, and serialize the attribute's data to XML-compliant formats.
/// </summary>
public sealed class BvAttribute
{
    private string _name = string.Empty;
    private string _value = string.Empty;

    public BvAttribute()
    {
    }

    public BvAttribute(string name, string value = "")
    {
        Name = name;
        Value = value;
    }

    public BvAttribute(BvAttribute other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Name = other.Name;
        Value = other.Value;
    }

    /// <summary>
    /// Gets or sets the name of the XML attribute. The name must be a valid XML-compliant attribute name,
    /// as determined by the <see cref="BvXmlValidator.IsValidName"/> method.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when an invalid name is assigned that does not conform to XML attribute naming rules.
    /// </exception>
    public string Name
    {
        get => _name;
        set
        {
            if (!BvXmlValidator.IsValidName(value))
                throw new ArgumentException("Invalid XML attribute name.", nameof(value));

            _name = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the XML attribute. The value must adhere to XML-compliant
    /// attribute value rules, which are validated by the <see cref="BvXmlValidator.IsValidValue"/> method.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when an invalid value is assigned that does not conform to XML attribute value constraints.
    /// </exception>
    public string Value
    {
        get => _value;
        set
        {
            if (!BvXmlValidator.IsValidValue(value))
                throw new ArgumentException("Invalid XML attribute value.", nameof(value));

            _value = value;
        }
    }

    /// <summary>
    /// Converts the current XML attribute into its XML-compliant string representation.
    /// </summary>
    /// <returns>
    /// A string representation of the XML attribute in the format `name="value"`,
    /// where the name is encoded as an XML-compliant name and the value is escaped to
    /// ensure XML compliance. Returns an empty string if the attribute name is null or empty.
    /// </returns>
    public string ToXml()
    {
        return string.IsNullOrEmpty(Name) 
            ? string.Empty 
            : $"{BvXmlEncoder.EncodeName(Name)}=\"{BvXmlEncoder.EscapeValue(Value)}\"";
    }

    /// <summary>
    /// Returns a string representation of the current XML attribute that is suitable
    /// for XML formatting. The result includes the name and value formatted as an
    /// XML attribute string in the form `name="value"`.
    /// </summary>
    /// <returns>
    /// A string representation of the XML attribute, or an empty string if the name
    /// is null or empty.
    /// </returns>
    public override string ToString() => ToXml();

    /// <summary>
    /// Creates a new instance of the <see cref="BvAttribute"/> class that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="BvAttribute"/> instance with the same name and value as the current instance.
    /// </returns>
    public BvAttribute Clone() => new(this);

    // Convenience value-related methods
    
    public int GetInt32()
        => int.Parse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture);

    public long GetInt64()
        => long.Parse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture);

    public double GetDouble()
        => double.Parse(Value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

    public decimal GetDecimal()
        => decimal.Parse(Value, NumberStyles.Number, CultureInfo.InvariantCulture);

    public bool GetBoolean()
        => bool.Parse(Value);

    public DateTime GetDateTime()
        => DateTime.Parse(Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    public DateTimeOffset GetDateTimeOffset()
        => DateTimeOffset.Parse(Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    public Guid GetGuid()
        => Guid.Parse(Value);

    public TEnum GetEnum<TEnum>() where TEnum : struct, Enum
        => Enum.Parse<TEnum>(Value, ignoreCase: false);

    // convenience factory methods
    
    public static BvAttribute Create(string name, string value)
        => new(name, value);

    public static BvAttribute Create(string name, int value)
        => new(name, value.ToString(CultureInfo.InvariantCulture));

    public static BvAttribute Create(string name, long value)
        => new(name, value.ToString(CultureInfo.InvariantCulture));

    public static BvAttribute Create(string name, double value)
        => new(name, value.ToString(CultureInfo.InvariantCulture));

    public static BvAttribute Create(string name, decimal value)
        => new(name, value.ToString(CultureInfo.InvariantCulture));

    public static BvAttribute Create(string name, bool value)
        => new(name, XmlConvert.ToString(value));

    public static BvAttribute Create(string name, DateTime value)
        => new(name, XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind));

    public static BvAttribute Create(string name, DateTimeOffset value)
        => new(name, XmlConvert.ToString(value));

    public static BvAttribute Create(string name, Guid value)
        => new(name, value.ToString("D"));

    public static BvAttribute Create<TEnum>(string name, TEnum value) where TEnum : struct, Enum
        => new(name, value.ToString());
}