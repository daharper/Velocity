using Velocity.Service.Core;

var service = XmppService.CreateBuilder(args)
    .UseTcpTransport()
    .Build();

await service.RunAsync();