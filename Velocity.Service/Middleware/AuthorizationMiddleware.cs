using Microsoft.Extensions.Logging;
using Velocity.Service.Identity;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Middleware;

public sealed class AuthorizationMiddleware : IXmppMiddleware
{
    private readonly AuthorizationCache _authorizationCache;
    private readonly ILogger<AuthorizationMiddleware> _logger;

    public AuthorizationMiddleware(AuthorizationCache authorizationCache, ILogger<AuthorizationMiddleware> logger)
    {
        _authorizationCache = authorizationCache;
        _logger = logger;
    }

    public async ValueTask InvokeAsync(
        XmppContext context,
        XmppDelegate next)
    {
        var jid = GetJid(context);

        var authorization = await _authorizationCache.GetAsync(jid, context.CancellationToken);

        context.Authorization = authorization;

        _logger.LogDebug(
            "Authorization: loaded profile for {Jid} with primary role {PrimaryRole}",
            authorization.Jid,
            authorization.PrimaryRole);

        await next(context);
    }

    private static string GetJid(XmppContext context)
    {
        // Temporary until stanza object exists.
        return "unknown";
    }
}