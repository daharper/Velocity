namespace Velocity.Service.Identity;

public sealed record AuthorizationProfile(string Jid, string PrimaryRole, IReadOnlyList<GroupRole> GroupRoles);