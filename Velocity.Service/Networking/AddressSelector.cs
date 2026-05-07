using System.Net;
using System.Net.Sockets;

namespace Velocity.Service.Networking;

/// <summary>
/// Represents a selector for selecting network addresses based on a specified  <see cref="AddressFamilyPreference"/>. 
/// </summary>
public sealed class AddressSelector : IAddressSelector
{
    /// <summary>
    /// Selects and orders a list of IP addresses based on the specified address family preference.
    /// </summary>
    /// <param name="addresses">A list of IP addresses to be filtered and sorted.</param>
    /// <param name="preference">The preferred address family order or filter to apply.</param>
    /// <returns>A read-only list of IP addresses filtered and ordered according to the specified preference.</returns>
    public IReadOnlyList<IPAddress> Select(IReadOnlyList<IPAddress> addresses, AddressFamilyPreference preference)
    {
        return preference switch
        {
            AddressFamilyPreference.PreferIPv6 =>
                addresses.OrderByDescending(a => a.AddressFamily == AddressFamily.InterNetworkV6).ToArray(),

            AddressFamilyPreference.PreferIPv4 =>
                addresses.OrderByDescending(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray(),

            AddressFamilyPreference.IPv6Only =>
                addresses.Where(a => a.AddressFamily == AddressFamily.InterNetworkV6).ToArray(),

            AddressFamilyPreference.IPv4Only =>
                addresses.Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray(),
            
            _ => addresses.ToArray()
        };
    }
}