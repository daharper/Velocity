using Velocity.Service.Core;
using Velocity.Service.Middleware;

var service = XmppService.CreateBuilder(args)
    .UseTcpTransport()
    .UseXmpp()
    .UseMiddleware<ExceptionHandlingMiddleware>()
    .UseMiddleware<LoggingMiddleware>()
    .UseMiddleware<RouterMiddleware>()
    .Build();

await service.RunAsync();