namespace Velocity.Service.Identity;

public interface IAuthorizationProvider
{
    ValueTask<AuthorizationProfile> GetAuthorizationAsync(string jid, CancellationToken cancellationToken);
}