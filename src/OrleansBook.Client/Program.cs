using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using OrleansBook.GrainInterfaces;
using System;
using System.Threading.Tasks;

namespace OrleansBook.Client;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = new HostBuilder().UseOrleansClient();

        var host = builder.Build();

        var client = host.Services.GetRequiredService<IClusterClient>();

        while (true)
        {
            Console.WriteLine("Please enter a robot name...");
            var grainId = Console.ReadLine();
            var grain = client.GetGrain<IRobotGrain>(grainId);
            Console.WriteLine("Please enter an instruction...");
            var instruction = Console.ReadLine();
            await grain.AddInstruction(instruction);
            var count = await grain.GetInstructionCount();
            Console.WriteLine($"{grainId} has {count} instruction(s)");
        }
    }
}