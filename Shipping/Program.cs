using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace Shipping
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Shipping";
            await Host.CreateDefaultBuilder(args)
                .UseNServiceBus(context =>
                {
                    var endpointConfiguration = new EndpointConfiguration("Shipping");
                    endpointConfiguration.UseTransport<LearningTransport>();

                    endpointConfiguration.UsePersistence<LearningPersistence>();
                    return endpointConfiguration;
                })
                .RunConsoleAsync();
        }
    }
}
