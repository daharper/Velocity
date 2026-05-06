using System.Net;
using System.Net.Sockets;

namespace Velocity.Service.Networking;

public sealed class AddressSelector : IAddressSelector
{
    public IReadOnlyList<IPAddress> Select(
        IReadOnlyList<IPAddress> addresses,
        AddressFamilyPreference preference)
    {
        return preference switch
        {
            AddressFamilyPreference.PreferIPv6 =>
                addresses
                    .OrderByDescending(a => a.AddressFamily == AddressFamily.InterNetworkV6)
                    .ToArray(),

            AddressFamilyPreference.PreferIPv4 =>
                addresses
                    .OrderByDescending(a => a.AddressFamily == AddressFamily.InterNetwork)
                    .ToArray(),

            AddressFamilyPreference.IPv6Only =>
                addresses
                    .Where(a => a.AddressFamily == AddressFamily.InterNetworkV6)
                    .ToArray(),

            AddressFamilyPreference.IPv4Only =>
                addresses
                    .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                    .ToArray(),

            AddressFamilyPreference.SystemOrder =>
                addresses.ToArray(),

            _ =>
                addresses.ToArray()
        };
    }
}