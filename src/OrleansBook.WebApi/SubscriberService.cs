
using Orleans.Streams;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.WebApi;

public class SubscriberService : IHostedService
{
    private readonly IClusterClient _client;
    private IAsyncStream<InstructionMessage> _stream;

    public SubscriberService(IClusterClient client)
    {
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _stream = _client.GetStreamProvider("StreamProvider")
            .GetStream<InstructionMessage>("StartingInstruction", Guid.Empty);
        await _stream.SubscribeAsync(new StreamSubscriber());
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
