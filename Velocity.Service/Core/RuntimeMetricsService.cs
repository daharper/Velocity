using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Core;

public sealed class RuntimeMetricsService : BackgroundService
{
    private readonly ILogger<RuntimeMetricsService> _logger;
    private readonly XmppChannelMetrics _channelMetrics;
    
    public RuntimeMetricsService(ILogger<RuntimeMetricsService> logger, XmppChannelMetrics channelMetrics)
    {
        _logger = logger;
        _channelMetrics = channelMetrics;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            long managedMemory = GC.GetTotalMemory(forceFullCollection: false);

            _logger.LogInformation(
                "Runtime metrics: managedMemoryBytes={ManagedMemoryBytes}, gen0={Gen0}, gen1={Gen1}, gen2={Gen2} inboundDepth={InboundDepth}, outboundDepth={OutboundDepth}",
                managedMemory,
                GC.CollectionCount(0),
                GC.CollectionCount(1),
                GC.CollectionCount(2),
                _channelMetrics.InboundDepth,
                _channelMetrics.OutboundDepth);
        }
    }
}