using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Logging;

namespace ReceiveWorker
{
    public class SimpleEventProcessorFactory : IEventProcessorFactory
    {
        private readonly ReceiveJournal _journal;
        private readonly ILogger<SimpleEventProcessor> _logger;

        public SimpleEventProcessorFactory(ReceiveJournal journal, ILogger<SimpleEventProcessor> logger)
        {
            _journal = journal;
            _logger = logger;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context) =>
            new SimpleEventProcessor(_journal, _logger);
    }
}
