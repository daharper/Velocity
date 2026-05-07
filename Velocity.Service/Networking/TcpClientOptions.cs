namespace Velocity.Service.Networking;

/// <summary>
/// Represents configurable options for establishing TCP client connections.
/// </summary>
public sealed class TcpClientOptions
{
    /// <summary>
    /// Specifies the hostname of the XMPP server to which a TCP connection should be established.
    /// </summary>
    public string Host { get; init; } = "";

    /// <summary>
    /// Specifies the port number on which the TCP connection should be established.
    /// </summary>
    public int Port { get; init; } = 5275;

    /// <summary>
    /// Defines the size, in bytes, of the buffer used for receiving data in TCP client connections.
    /// </summary>
    public int ReceiveBufferSize { get; init; } = 16 * 1024;

    /// <summary>
    /// Specifies the size, in bytes, of the send buffer to be used for outgoing TCP connections.
    /// A larger buffer size may improve performance for high-throughput data transfers.
    /// </summary>
    public int SendBufferSize { get; init; } = 16 * 1024;

    /// <summary>
    /// Specifies the maximum number of messages that can be queued in the inbound channel at a given time.
    /// This property determines the capacity of the channel used for processing incoming data streams,
    /// ensuring efficient handling of incoming connections without overwhelming system resources.
    /// </summary>
    public int InboundChannelCapacity { get; init; } = 4096;

    /// <summary>
    /// Defines the maximum number of messages that can be queued in the outbound communication channel
    /// before the channel begins to block new writes, awaiting space to become available.
    /// </summary>
    public int OutboundChannelCapacity { get; init; } = 4096;

    /// <summary>
    /// Specifies the name of the XMPP component to which the connection is being established.
    /// </summary>
    public string ComponentName { get; init; } = "";

    /// <summary>
    /// Defines the preferred address family to use when selecting network endpoints
    /// from a list of resolved IP addresses.
    /// </summary>
    public AddressFamilyPreference AddressFamilyPreference { get; init; }

    /// <summary>
    /// Specifies the maximum duration allowed for establishing a TCP connection
    /// before the operation is considered to have timed out.
    /// </summary>
    public TimeSpan ConnectTimeout { get; init; }

    /// <summary>
    /// Defines the duration for which DNS resolution results should be cached.
    /// A non-positive value indicates that DNS caching is disabled.
    /// </summary>
    public TimeSpan DnsCacheDuration { get; init; }

    /// <summary>
    /// Defines the delay duration before attempting to establish a connection using fallback mechanisms
    /// when the initial connection attempt fails.
    /// </summary>
    public TimeSpan FallbackDelay { get; init; }

    /// <summary>
    /// Specifies the maximum number of reconnection attempts allowed when a TCP connection is lost.
    /// A value of <c>null</c> indicates that there is no limit on the number of reconnection attempts.
    /// </summary>
    public int? MaxReconnectAttempts { get; init; } = null;
    
    /// <summary>
    /// Specifies the amount of time to wait before attempting to reconnect to a server
    /// after a connection is lost or a connection attempt fails.
    /// </summary>
    public TimeSpan ReconnectDelay { get; init; } = TimeSpan.FromSeconds(5);
}