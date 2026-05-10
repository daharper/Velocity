using System.Text;

namespace Velocity.Foundation.Xml;

using System.Dynamic;

/// <summary>
/// Represents a dynamic XML-like element with a name, value, attributes, and child elements.
/// Provides functionality for manipulating and querying attributes and child elements as well
/// as XML serialization.
/// </summary>
public class BvElement : DynamicObject
{
    private readonly List<BvElement> _elems = [];

    public BvElement(string name, string value = "")
    {
        if (!BvXmlValidator.IsValidName(name))
            throw new ArgumentException("Invalid XML element name.", nameof(name));

        Name = name;
        Value = value;
    }

    public BvElement(BvElement other)
    {
        ArgumentNullException.ThrowIfNull(other);

        Name = other.Name;
        Value = other.Value;
        
        foreach (var attribute in other.Attributes)
            Attributes.AddUnchecked(attribute);

        _elems.AddRange(other.Elems);
    }
    
    public string this[string name]
    {
        get => Attributes[name];
        set => Attributes[name] = value;
    }

    public string Name { get; }

    public string Value { get; set; }

    public BvAttributeCollection Attributes { get; } = [];

    public IReadOnlyList<BvElement> Elems => _elems;

    public bool HasAttrs => Attributes.Count > 0;

    public bool HasElems => _elems.Count > 0;
    
    public BvElement Clone() => new(this);
    
    public BvAttribute Attr(string name)
        => Attributes.Attr(name);

    public BvAttribute Attr(string name, string defaultValue) 
        => Attributes.Attr(name, defaultValue);

    public BvElement Elem(string name) 
        => Elem(name, string.Empty);

    public BvElement Elem(string name, string defaultValue)
    {
        var existing = FindElem(name);

        if (existing is not null)
            return existing;

        var elem = new BvElement(name)
        {
            Value = defaultValue
        };

        _elems.Add(elem);
        return elem;
    }

    public BvElement Add(BvElement elem)
    {
        ArgumentNullException.ThrowIfNull(elem);

        _elems.Add(elem);
        
        return this;
    }

    public BvElement Add(string name)
    {
        var elem = new BvElement(name);
        
        _elems.Add(elem);
        
        return elem;
    }
    
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = Elem(NormalizeDynamicName(binder.Name));
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        var elem = Elem(NormalizeDynamicName(binder.Name));

        elem.Value = value?.ToString() ?? string.Empty;

        return true;
    }

    public BvElement? FindElem(string name)
    {
        foreach (var elem in _elems)
        {
            if (string.Equals(elem.Name, name, StringComparison.Ordinal))
                return elem;
        }

        return null;
    }
    
    public string ToXml()
    {
        var sb = new StringBuilder();
        WriteXml(sb);
        return sb.ToString();
    }   
    
    protected virtual void WriteXml(StringBuilder sb)
    {
        sb.Append('<');
        sb.Append(BvXmlEncoder.EncodeName(Name));

        foreach (var attribute in Attributes)
        {
            var xml = attribute.ToXml();

            if (!string.IsNullOrEmpty(xml))
            {
                sb.Append(' ');
                sb.Append(xml);
            }
        }

        if (string.IsNullOrEmpty(Value) && _elems.Count == 0)
        {
            sb.Append(" />");
            return;
        }

        sb.Append('>');

        if (!string.IsNullOrEmpty(Value))
            sb.Append(BvXmlEncoder.EscapeValue(Value));

        foreach (var elem in _elems)
            elem.WriteXml(sb);

        sb.Append("</");
        sb.Append(BvXmlEncoder.EncodeName(Name));
        sb.Append('>');
    }

    public override string ToString() => ToXml();    
    
    private static string NormalizeDynamicName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        return char.ToLowerInvariant(name[0]) + name[1..];
    }
}