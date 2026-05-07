namespace Velocity.Service.Networking;

/// <summary>
/// Specifies the preferred address family when selecting network endpoints from a list of resolved addresses.
/// </summary>
public enum AddressFamilyPreference
{
    /// <summary>
    /// Specifies that the system's default ordering of IP addresses should be used,
    /// without any preference or filtering applied to select between IPv4 or IPv6 addresses.
    /// </summary>
    SystemOrder,

    /// <summary>
    /// Specifies a preference for IPv6 addresses when selecting network endpoints,
    /// while still allowing IPv4 addresses if no suitable IPv6 address is available.
    /// </summary>
    PreferIPv6,

    /// <summary>
    /// Specifies a preference for selecting IPv4 (Internet Protocol version 4) addresses
    /// when resolving or ordering network endpoints from a list of available addresses.
    /// </summary>
    PreferIPv4,

    /// <summary>
    /// Specifies that only IPv6 addresses should be used when selecting network endpoints.
    /// Any IPv4 addresses will be excluded from consideration.
    /// </summary>
    IPv6Only,

    /// <summary>
    /// Specifies that only IPv4 addresses should be selected, excluding any IPv6 addresses.
    /// </summary>
    IPv4Only
}