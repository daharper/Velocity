using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Velocity.Service.Networking;
using Velocity.Service.Transport;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Core;

/// <summary>
/// Provides a builder class for configuring and creating XMPP services.
/// </summary>
public sealed class XmppServiceBuilder
{
    internal HostApplicationBuilder HostBuilder { get; }

    private readonly List<Type> _middlewareTypes = [];
    
    internal XmppServiceBuilder(string[] args)
    {
        HostBuilder = Host.CreateApplicationBuilder(args);
    }
    
    public XmppServiceBuilder UseTcpTransport()
    {
        HostBuilder.Services.Configure<TcpClientOptions>(HostBuilder.Configuration.GetSection("TcpClient"));
        HostBuilder.Services.AddHybridCache();
        HostBuilder.Services.AddSingleton<IDnsResolver, DnsResolver>();
        HostBuilder.Services.AddSingleton<IAddressSelector, AddressSelector>();
        HostBuilder.Services.AddSingleton<TcpConnector>();
        HostBuilder.Services.AddSingleton<ITransportConnector, TcpTransportConnector>();

        return this;
    }
    
    public XmppServiceBuilder UseXmpp()
    {
        HostBuilder.Services.AddSingleton<IXmppStreamParser, XmppStreamParser>();
        HostBuilder.Services.AddSingleton<IXmppResponseWriter, XmppResponseWriter>();
        HostBuilder.Services.AddSingleton<XmppOutputWriter>();
        HostBuilder.Services.AddSingleton<IXmppOutputWriter>(sp => sp.GetRequiredService<XmppOutputWriter>());
        HostBuilder.Services.AddSingleton<IXmppPipeline, XmppPipeline>();
        HostBuilder.Services.AddSingleton<IXmppConnectionHandler, XmppConnectionHandler>();
        
        return this;
    }
    
    public XmppServiceBuilder UseMiddleware<TMiddleware>() where TMiddleware : class, IXmppMiddleware
    {
        _middlewareTypes.Add(typeof(TMiddleware));
        
        return this;
    }
    
    /// <summary>
    /// Builds and returns an instance of the <see cref="XmppService"/> using the current configuration settings.
    /// </summary>
    /// <returns>A fully configured <see cref="XmppService"/> instance.</returns>
    public XmppService Build()
    {
        foreach (var middlewareType in _middlewareTypes)
        {
            HostBuilder.Services.AddSingleton(typeof(IXmppMiddleware), middlewareType);
        }

        var host = HostBuilder.Build();

        return new XmppService(host);
    }
}