namespace Velocity.Service.Networking;

/// <summary>
/// Specifies the preferred address family when selecting network endpoints from a list of resolved addresses.
/// </summary>
public enum AddressFamilyPreference
{
    SystemOrder,
    PreferIPv6,
    PreferIPv4,
    IPv6Only,
    IPv4Only
}