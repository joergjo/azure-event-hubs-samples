using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ReceiveWorker
{
    public class Worker : BackgroundService
    {
        private readonly EventProcessorHost _eventProcessorHost;
        private readonly IEventProcessorFactory _eventProcessorFactory;
        private readonly ILogger _logger;
        private readonly ReceiveJournal _journal;

        private const int HeartBeatMilliSeconds = 10_000;

        public Worker(
            IEventProcessorFactory eventProcessorFactory,
            ReceiveJournal journal,
            ILogger<Worker> logger, 
            IOptionsMonitor<SimpleEventProcessorOptions> optionsAccessor)
        {
            _eventProcessorFactory = eventProcessorFactory;
            _journal = journal;
            _logger = logger;
            var options = optionsAccessor.CurrentValue;

            _eventProcessorHost = new EventProcessorHost(
                options.EventHub,
                options.ConsumerGroup,
                options.EventHubConnectionString,
                options.StorageConnectionString,
                options.StorageContainer);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker starting at {Time}.", DateTimeOffset.UtcNow);
            await _eventProcessorHost.RegisterEventProcessorFactoryAsync(_eventProcessorFactory);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at {Time}.", DateTimeOffset.UtcNow);
                _logger.LogInformation(_journal.GetStatistics());
                await Task.Delay(HeartBeatMilliSeconds, cancellationToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker stopping at {Time}.", DateTimeOffset.UtcNow);
            await base.StopAsync(cancellationToken);
            await _eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
