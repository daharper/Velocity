using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Velocity.Service;
using Velocity.Service.Core;

var service = XmppService.CreateBuilder(args)
    .UseTcpTransport()
    .Build();

await service.RunAsync();
// var builder = Host.CreateApplicationBuilder(args);
//
// builder.Services.Configure<TcpClientOptions>(
//     builder.Configuration.GetSection("TcpClient"));
//
// builder.Services.AddHybridCache();
//
// builder.Services.AddSingleton<IDnsResolver, DnsResolver>();
// builder.Services.AddSingleton<IAddressSelector, AddressSelector>();
// builder.Services.AddSingleton<TcpConnector>();
//
// using var host = builder.Build();
//
// var options = host.Services.GetRequiredService<IOptions<TcpClientOptions>>().Value;
//
// var connector = host.Services.GetRequiredService<TcpConnector>();
//
// using var socket = await connector.ConnectAsync(options.Host, options.Port, CancellationToken.None);