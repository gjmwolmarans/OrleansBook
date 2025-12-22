using Orleans.Streams;
using OrleansBook.GrainInterfaces;
using OrleansBook.WebApi;

var builder = new HostBuilder()
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering();
        client.AddMemoryStreams("StreamProvider");
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });

var app = builder.Build();

app.Run();