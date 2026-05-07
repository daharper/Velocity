namespace Velocity.Service.Identity;

public sealed class StubAuthorizationProvider : IAuthorizationProvider
{
    public ValueTask<AuthorizationProfile> GetAuthorizationAsync(string jid, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        AuthorizationProfile profile = new(
            Jid: jid,
            PrimaryRole: "user",
            GroupRoles:
            [
                new GroupRole("default", "member")
            ]);

        return ValueTask.FromResult(profile);
    }
}