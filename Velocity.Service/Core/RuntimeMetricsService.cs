using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Velocity.Service.Xmpp;

namespace Velocity.Service.Core;

public sealed class RuntimeMetricsService : BackgroundService
{
    private readonly ILogger<RuntimeMetricsService> _logger;
    private readonly XmppChannelMetrics _channelMetrics;
    private readonly IXmppOutputWriter _outputWriter;
    
    public RuntimeMetricsService(XmppChannelMetrics channelMetrics, IXmppOutputWriter outputWriter, ILogger<RuntimeMetricsService> logger)
    {
        _channelMetrics = channelMetrics;
        _outputWriter = outputWriter;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var managedMemory = GC.GetTotalMemory(forceFullCollection: false);

            // todo - publish to observers
            // if (_outputWriter.IsAttached)
            // {
            //     await _outputWriter.WriteAsync(
            //         metricsXml,
            //         stoppingToken);
            // }
            
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