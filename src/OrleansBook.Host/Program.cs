using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Orleans.Serialization;
using OrleansBook.GrainClasses;

namespace OrleansBook.Host;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .UseOrleans(builder =>
            {
                builder
                    .UseDashboard()
                    .AddMemoryStreams("StreamProvider")
                    .AddMemoryGrainStorageAsDefault()
                    .AddMemoryGrainStorage("robotStateStore")
                    .AddMemoryGrainStorage("PubSubStore");
                builder.ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                });
                builder.UseLocalhostClustering();
            })
            .Build();
        await host.StartAsync();
        Console.WriteLine("Silo started. Press Enter to terminate...");
        Console.ReadLine();
        await host.StopAsync();
    }
}