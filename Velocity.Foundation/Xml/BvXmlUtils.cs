using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Velocity.Foundation.Xml;

/// <summary>
/// A utility class for handling XML serialization and deserialization, as well as providing
/// additional XML-related functionality such as saving, loading, and pretty-printing XML content.
/// </summary>
public static class BvXmlUtils
{
    /// <summary>
    /// Serializes the specified object into its XML string representation.
    /// </summary>
    /// <param name="value">The object to serialize. It cannot be null.</param>
    /// <param name="defaultNamespace">
    /// The default XML namespace to use during serialization.
    /// If null or empty, no namespace will be applied.
    /// </param>
    /// <returns>A string containing the XML representation of the serialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="value"/> parameter is null.</exception>
    public static string ToXml(object value, string? defaultNamespace = null)
    {
        ArgumentNullException.ThrowIfNull(value);

        var serializer = string.IsNullOrWhiteSpace(defaultNamespace)
            ? new XmlSerializer(value.GetType())
            : new XmlSerializer(value.GetType(), defaultNamespace);

        var settings = new XmlWriterSettings
        {
            Indent = false,
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        };

        using var writer = new Utf8StringWriter();

        using (var xmlWriter = XmlWriter.Create(writer, settings))
        {
            serializer.Serialize(xmlWriter, value);
        }

        return writer.ToString();
    }

    /// <summary>
    /// Deserializes the specified XML string into an object of the specified type.
    /// </summary>
    /// <param name="xml">The XML string to deserialize. It cannot be null or empty.</param>
    /// <param name="defaultNamespace">
    /// The default XML namespace used during deserialization.
    /// If null or empty, no namespace will be applied.
    /// </param>
    /// <typeparam name="T">The type of the object to be deserialized. Must be a reference type.</typeparam>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="xml"/> parameter is null or whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when deserialization fails or the XML cannot be converted to the specified type.
    /// </exception>
    public static T FromXml<T>(string xml, string? defaultNamespace = null) where T : class
    {
        if (string.IsNullOrWhiteSpace(xml))
            throw new ArgumentException("XML cannot be empty.", nameof(xml));

        var serializer = string.IsNullOrWhiteSpace(defaultNamespace)
            ? new XmlSerializer(typeof(T))
            : new XmlSerializer(typeof(T), defaultNamespace);

        using var reader = new StringReader(xml);

        return serializer.Deserialize(reader) as T
            ?? throw new InvalidOperationException(
                $"XML could not be deserialized as {typeof(T).FullName}.");
    }

    /// <summary>
    /// Saves the specified object as an XML file at the given file path.
    /// </summary>
    /// <param name="path">
    /// The file path where the XML content will be saved. It cannot be null, empty, or whitespace.
    /// </param>
    /// <param name="value">
    /// The object to serialize and save as XML. It cannot be null.
    /// </param>
    /// <param name="defaultNamespace">
    /// The default XML namespace to use during serialization. If null or empty, no namespace will be applied.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="path"/> is null, empty, or contains only whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="value"/> parameter is null.
    /// </exception>
    public static void Save(string path, object value, string? defaultNamespace = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path cannot be blank.", nameof(path));

        var xml = ToXml(value, defaultNamespace);
        File.WriteAllText(path, xml, Encoding.UTF8);
    }

    /// <summary>
    /// Loads and deserializes the XML content from the specified file path into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize the XML content into. It must be a reference type.</typeparam>
    /// <param name="path">The file path to the XML file. It cannot be null, empty, or whitespace.</param>
    /// <param name="defaultNamespace">
    /// The default XML namespace to use during deserialization. If null or empty, no namespace will be applied.
    /// </param>
    /// <returns>An object of type <typeparamref name="T"/> deserialized from the XML content of the file.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="path"/> parameter is null, empty, or contains only whitespace.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the deserialization process fails or the XML content cannot be converted to the specified type <typeparamref name="T"/>.
    /// </exception>
    public static T Load<T>(string path, string? defaultNamespace = null)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("File path cannot be blank.", nameof(path));

        var xml = File.ReadAllText(path, Encoding.UTF8);
        return FromXml<T>(xml, defaultNamespace);
    }

    /// <summary>
    /// Formats an XML string to ensure proper indentation and readability.
    /// </summary>
    /// <param name="xml">The XML string to format. It must be a valid XML document.</param>
    /// <returns>
    /// The formatted and indented XML string, or an empty string if the input is null or whitespace.
    /// </returns>
    /// <exception cref="XmlException">Thrown when the <paramref name="xml"/> parameter is not a valid XML document.</exception>
    public static string PrettyPrintXml(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return string.Empty;

        var document = new XmlDocument
        {
            XmlResolver = null
        };

        document.LoadXml(xml);

        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "\t"
        };

        using var writer = new Utf8StringWriter();

        using (var xmlWriter = XmlWriter.Create(writer, settings))
        {
            document.WriteTo(xmlWriter);
        }

        return writer.ToString();
    }

    /// <summary>
    /// A specialized StringWriter implementation that overrides the default encoding to UTF-8.
    /// This class is primarily used to ensure that written text is encoded using the UTF-8 character set,
    /// which is particularly useful in XML serialization scenarios requiring consistent character encoding.
    /// </summary>
    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}