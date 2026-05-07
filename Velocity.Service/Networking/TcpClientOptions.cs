namespace Velocity.Service.Networking;

/// <summary>
/// Represents configurable options for establishing TCP client connections.
/// </summary>
public sealed class TcpClientOptions
{
    public string Host { get; init; } = "";
   
    public int Port { get; init; }
    
    public string ComponentName { get; init; } = "";
    
    public AddressFamilyPreference AddressFamilyPreference { get; init; }
    
    public TimeSpan ConnectTimeout { get; init; }
    
    public TimeSpan DnsCacheDuration { get; init; }
    
    public TimeSpan FallbackDelay { get; init; }
}