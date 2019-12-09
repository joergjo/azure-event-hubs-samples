using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Logging;

namespace ReceiveWorker
{
    public class SimpleEventProcessor : IEventProcessor
    {
        private readonly ILogger _logger;
        private readonly ReceiveJournal _journal;

        public SimpleEventProcessor(ReceiveJournal journal, ILogger<SimpleEventProcessor> logger)
        {
            _journal = journal;
            _logger = logger;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            _logger.LogInformation(
                "SimpleEventProcessor closing for partition '{PartitionId}'.",
                context.PartitionId);
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            _logger.LogInformation(
                "SimpleEventProcessor opening for partition '{PartitionId}'.",
                context.PartitionId);
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            _logger.LogError(
                "Error on partiton: '{PartitionId}'. Error: {Message}.", 
                context.PartitionId, 
                error.Message);
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var message in messages)
            {
                _logger.LogDebug("Message received on partition '{PartitionId}'.", context.PartitionId);
                _journal.AddOrUpdate(context.PartitionId, 1, (key, val) => val + 1);
            }
            return context.CheckpointAsync();
        }
    }
}
