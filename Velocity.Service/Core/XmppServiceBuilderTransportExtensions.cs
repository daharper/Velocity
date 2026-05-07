using Microsoft.Extensions.DependencyInjection;
using Velocity.Service.Middleware;
using Velocity.Service.Networking;
using Velocity.Service.Transport;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Core;

/// <summary>
/// Provides extension methods for configuring transport options in the XMPP service builder.
/// </summary>
public static class XmppServiceBuilderTransportExtensions
{
    public static XmppServiceBuilder UseTcpTransport(this XmppServiceBuilder builder)
    {
        builder.HostBuilder.Services.Configure<TcpClientOptions>(
            builder.HostBuilder.Configuration.GetSection("TcpClient"));

        builder.HostBuilder.Services.AddHybridCache();

        builder.HostBuilder.Services.AddSingleton<IDnsResolver, DnsResolver>();
        builder.HostBuilder.Services.AddSingleton<IAddressSelector, AddressSelector>();
        builder.HostBuilder.Services.AddSingleton<TcpConnector>();

        builder.HostBuilder.Services.AddSingleton<ITransportConnector, TcpTransportConnector>();
        builder.HostBuilder.Services.AddSingleton<IXmppConnectionHandler, XmppConnectionHandler>();
        builder.HostBuilder.Services.AddSingleton<IXmppStreamParser, XmppStreamParser>();
        
        builder.HostBuilder.Services.AddSingleton<XmppOutputWriter>();
        builder.HostBuilder.Services.AddSingleton<IXmppOutputWriter>(sp => sp.GetRequiredService<XmppOutputWriter>());
        
        builder.HostBuilder.Services.AddSingleton<IXmppPipeline, XmppPipeline>();
        builder.HostBuilder.Services.AddSingleton<IXmppMiddleware, LoggingMiddleware>();
        builder.HostBuilder.Services.AddSingleton<IXmppMiddleware, RouterMiddleware>();
        builder.HostBuilder.Services.AddSingleton<IXmppMiddleware, EndPointMiddleware>();        
        
        return builder;
    }
}