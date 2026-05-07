using System.Net;

namespace Velocity.Service.Networking;

/// <summary>
/// Represents a contract for selecting a subset of IP addresses from a given list based on specified preferences.
/// </summary>
public interface IAddressSelector
{
    /// <summary>
    /// Selects a subset of IP addresses from a given list based on the specified address family preference.
    /// </summary>
    /// <param name="addresses">The list of IP addresses to filter.</param>
    /// <param name="preference">
    /// The address family preference used to determine the selection criteria for filtering IP addresses.
    /// Possible values include options to prioritize IPv4, IPv6, or system-defined orders.
    /// </param>
    /// <returns>
    /// A filtered, read-only list of IP addresses that match the specified preference. Returns an empty list
    /// if no addresses match the criteria or if the input list is empty.
    /// </returns>
    IReadOnlyList<IPAddress> Select(IReadOnlyList<IPAddress> addresses, AddressFamilyPreference preference);
}