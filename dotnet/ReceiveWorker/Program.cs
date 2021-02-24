using System;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ReceiveWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddSingleton<IEventProcessorFactory, SimpleEventProcessorFactory>();
                    services.AddSingleton(new ReceiveJournal());
                    services.AddOptions<HostOptions>().Configure(
                        options => options.ShutdownTimeout = TimeSpan.FromSeconds(60));
                    services
                        .AddOptions<SimpleEventProcessorOptions>()
                        .Bind(hostContext.Configuration.GetSection(nameof(SimpleEventProcessor)))
                        .PostConfigure(options =>
                        {
                            if (string.IsNullOrEmpty(options.ConsumerGroup))
                            {
                                options.ConsumerGroup = PartitionReceiver.DefaultConsumerGroupName;
                            }
                            if (string.IsNullOrEmpty(options.StorageContainer))
                            {
                                options.StorageContainer = nameof(ReceiveWorker).ToLowerInvariant();
                            }
                        })
                        .ValidateDataAnnotations();
                });
    }
}
