using System.ComponentModel.DataAnnotations;

namespace ReceiveWorker
{
    public class SimpleEventProcessorOptions
    {
        [Required]
        public string? EventHubConnectionString { get; set; }
        
        [Required]
        public string? EventHub { get; set; }
        
        public string? ConsumerGroup { get; set; }
        
        [Required]
        public string? StorageConnectionString { get; set; }

        public string? StorageContainer { get; set; }
    }
}
