using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses;

[ImplicitStreamSubscription("StartingInstruction")]
public class SubscriberGrain :
    Grain, ISubscriberGrain, IAsyncObserver<InstructionMessage>
{

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var key = this.GetPrimaryKey();
        await this.GetStreamProvider("StreamProvider")
            .GetStream<InstructionMessage>("StartingInstruction", Guid.Empty)
            .SubscribeAsync(this);
        await base.OnActivateAsync(cancellationToken);
    }

    public Task OnCompletedAsync() => Task.CompletedTask;

    public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

    public Task OnNextAsync(InstructionMessage instruction, StreamSequenceToken? token = null)
    {
        var msg = $"{instruction.Robot} starting \"{instruction.Instruction}\"";
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }
}