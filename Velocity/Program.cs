using Velocity.Service.Core;
using Velocity.Service.Identity;
using Velocity.Service.Middleware;

var service = XmppService.CreateBuilder(args)
    .UseTcpTransport()
    .UseXmpp()
    .AddSingleton<IAuthorizationProvider, StubAuthorizationProvider>()
    .UseMiddleware<ExceptionHandlingMiddleware>()
    .UseMiddleware<LoggingMiddleware>()
    .UseMiddleware<RouterMiddleware>()
    .UseMiddleware<AuthorizationMiddleware>()
    .UseMiddleware<EndpointMiddleware>()
    .Build();

await service.RunAsync();