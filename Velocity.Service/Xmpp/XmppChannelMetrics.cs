namespace Velocity.Service.Xmpp;

public sealed class XmppChannelMetrics
{
    private long _inboundDepth;
    private long _outboundDepth;

    public long InboundDepth => Interlocked.Read(ref _inboundDepth);
    public long OutboundDepth => Interlocked.Read(ref _outboundDepth);

    public void InboundQueued() => Interlocked.Increment(ref _inboundDepth);
    public void InboundDequeued() => Interlocked.Decrement(ref _inboundDepth);

    public void OutboundQueued() => Interlocked.Increment(ref _outboundDepth);
    public void OutboundDequeued() => Interlocked.Decrement(ref _outboundDepth);
}