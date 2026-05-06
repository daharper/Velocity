using Microsoft.Extensions.Hosting;

namespace Velocity.Service.Core;

/// <summary>
/// Provides a builder class for configuring and creating XMPP services.
/// </summary>
public sealed class XmppServiceBuilder
{
    internal HostApplicationBuilder HostBuilder { get; }

    internal XmppServiceBuilder(string[] args)
    {
        HostBuilder = Host.CreateApplicationBuilder(args);
    }

    /// <summary>
    /// Builds and returns an instance of the <see cref="XmppService"/> using the current configuration settings.
    /// </summary>
    /// <returns>A fully configured <see cref="XmppService"/> instance.</returns>
    public XmppService Build()
    {
        var host = HostBuilder.Build();
        return new XmppService(host);
    }
}