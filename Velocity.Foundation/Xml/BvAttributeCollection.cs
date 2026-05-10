using System.Collections;

namespace Velocity.Foundation.Xml;

/// <summary>
/// Represents a collection of <see cref="BvAttribute"/> objects that can be accessed and manipulated
/// by name, providing functionality for working with XML-like attribute data.
/// </summary>
public sealed class BvAttributeCollection : IEnumerable<BvAttribute>
{
    private readonly List<BvAttribute> _items = [];
    
    /// <summary>
    /// Provides indexed access to the <see cref="BvAttribute"/> instances in the collection
    /// using the attribute name as a key. Allows getting and setting the value of an attribute
    /// by its name. If the attribute does not exist when setting a value, it will be created.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <returns>The value of the attribute with the specified name.</returns>
    public string this[string name]
    {
        get => Attr(name).Value;
        set => Attr(name).Value = value;
    }
    
    /// <summary>
    /// Gets the number of <see cref="BvAttribute"/> objects contained in the collection.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Retrieves a <see cref="BvAttribute"/> from the collection with the specified name.
    /// If no attribute is found with the given name, a new <see cref="BvAttribute"/> with the provided
    /// name and an empty value is created and returned.
    /// </summary>
    /// <param name="name">The name of the attribute to retrieve or create.</param>
    /// <returns>A <see cref="BvAttribute"/> object associated with the specified name.</returns>
    public BvAttribute Attr(string name) => Attr(name, string.Empty);

    /// <summary>
    /// Retrieves a <see cref="BvAttribute"/> from the collection with the specified name and default value.
    /// If no attribute is found with the given name, a new <see cref="BvAttribute"/> with the provided
    /// name and default value is created and returned.
    /// </summary>
    /// <param name="name">The name of the attribute to retrieve or create.</param>
    /// <param name="defaultValue">The default value to assign to the attribute if it does not already exist.</param>
    /// <returns>A <see cref="BvAttribute"/> object associated with the specified name and default value.</returns>
    public BvAttribute Attr(string name, string defaultValue)
    {
        var existing = Find(name);

        if (existing is not null)
            return existing; 

        var created = new BvAttribute(name, defaultValue);
        _items.Add(created);

        return created;
    }

    /// <summary>
    /// Attempts to retrieve a <see cref="BvAttribute"/> from the collection with the specified name.
    /// If an attribute with the given name exists in the collection, it is returned, and the method
    /// returns true. Otherwise, the method returns false, and the output parameter is set to null.
    /// </summary>
    /// <param name="name">The name of the attribute to search for in the collection.</param>
    /// <param name="attribute">The value to be set if the attribute is found.</param>
    /// <returns>
    /// True if an attribute with the specified name exists in the collection; otherwise, false.
    /// </returns>
    public bool TryGet(string name, out BvAttribute attribute)
    {
        var existing = Find(name);

        if (existing is not null)
        {
            attribute = existing;
            return true;
        }

        attribute = null!;
        return false;
    }

    /// <summary>
    /// Determines whether the collection contains a <see cref="BvAttribute"/> with the specified name.
    /// </summary>
    /// <param name="name">The name of the attribute to locate within the collection.</param>
    /// <returns>true if an attribute with the specified name exists in the collection; otherwise, false.</returns>
    public bool Contains(string name) => Find(name) is not null;

    /// <summary>Removes a <see cref="BvAttribute"/> from the collection based on the provided name.</summary>
    /// <param name="name">The name of the attribute to remove from the collection.</param>
    /// <returns>True if an attribute with the specified name was found and removed; otherwise, false.</returns>
    public bool Remove(string name)
    {
        var index = _items.FindIndex(x => string.Equals(x.Name, name, StringComparison.Ordinal));
        if (index < 0) return false;

        _items.RemoveAt(index);
        
        return true;
    }

    /// <summary>
    /// Searches for a <see cref="BvAttribute"/> in the collection with the specified name.
    /// If an attribute with the given name exists, it is returned; otherwise, null is returned.
    /// </summary>
    /// <param name="name">The name of the attribute to search for in the collection.</param>
    /// <returns>
    /// The <see cref="BvAttribute"/> object with the specified name if found; otherwise, null.
    /// </returns>
    private BvAttribute? Find(string name)
    {
        foreach (var attribute in _items)
        {
            if (string.Equals(attribute.Name, name, StringComparison.Ordinal))
                return attribute;
        }

        return null;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of <see cref="BvAttribute"/> objects.
    /// </summary>
    /// <returns>An enumerator for the collection of <see cref="BvAttribute"/>.</returns>
    public IEnumerator<BvAttribute> GetEnumerator() => _items.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection of <see cref="BvAttribute"/> objects.
    /// </summary>
    /// <returns>An enumerator for the collection of <see cref="BvAttribute"/>.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}