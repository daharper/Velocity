namespace Velocity.Service.Xmpp;

public interface IXmppStreamParser
{
    void Parse(ReadOnlySpan<char> chars, CancellationToken cancellationToken);
}