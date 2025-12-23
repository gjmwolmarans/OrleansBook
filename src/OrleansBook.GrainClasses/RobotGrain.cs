using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Reminders;
using OrleansBook.GrainInterfaces;
using Orleans.Transactions.Abstractions;

namespace OrleansBook.GrainClasses;

public class RobotGrain : Grain, IRobotGrain
{
    //int instructionsEnqueued = 0;
    //int instructionsDequeued = 0;
    private readonly ILogger<RobotGrain> _logger;
    private readonly ITransactionalState<RobotState> _state;
    //private IAsyncStream<InstructionMessage> _stream;

    public RobotGrain(ILogger<RobotGrain> logger,
        [TransactionalState("robotState", "robotStateStore")] ITransactionalState<RobotState> state)
    {
        _logger = logger;
        _state = state;
        //_stream = this.GetStreamProvider("StreamProvider")
        //    .GetStream<InstructionMessage>("StartingInstruction", Guid.Empty);
    }

    //async Task Publish(string instruction)
    //{
    //    var key = this.GetPrimaryKeyString();
    //    var message = new InstructionMessage(instruction, key);
    //    await _stream.OnNextAsync(message);
    //}

    public async Task AddInstruction(string instruction)
    {
        var key = this.GetPrimaryKeyString();
        _logger.LogWarning($"{key} adding '{instruction}'");
        await _state.PerformUpdate(state => state.Instructions.Enqueue(instruction));
        //instructionsEnqueued += 1;
        //await _state.WriteStateAsync();
    }

    public async Task<int> GetInstructionCount()
    {
        return await _state.PerformRead(state => state.Instructions.Count);
    }

    public async Task<string> GetNextInstruction()
    {
        var key = this.GetPrimaryKeyString();
        string instruction = null;
        await _state.PerformUpdate(state =>
        {
            if (state.Instructions.Count == 0) return;
            instruction = state.Instructions.Dequeue();
        });
        if (instruction is not null)
        {
            _logger.LogWarning($"{key} returning '{instruction}'");
        }
        
        //await Publish(instruction);
        //instructionsDequeued += 1;
        //await _state.WriteStateAsync();
        return instruction;
    }

    //public override Task OnActivateAsync(CancellationToken cancellationToken)
    //{
    //    var oneMinute = TimeSpan.FromMinutes(1);
    //    this.RegisterGrainTimer<object>(this.ResetsStats, null, oneMinute, oneMinute);
    //    var oneDay = TimeSpan.FromDays(1);
    //    this.RegisterOrUpdateReminder("firmware", oneDay, oneDay);
    //    return base.OnActivateAsync(cancellationToken);
    //}

    //private Task ResetsStats(object _, CancellationToken cancellationToken)
    //{
    //    var key = this.GetPrimaryKeyString();
    //    Console.WriteLine($"{key} enqueued: {instructionsEnqueued}");
    //    Console.WriteLine($"{key} dequeued: {instructionsDequeued}");
    //    Console.WriteLine($"{key} queued: {_state.State.Instructions.Count}");
    //    instructionsEnqueued = 0;
    //    instructionsDequeued = 0;
    //    return Task.CompletedTask;
    //}

    //public Task ReceiveReminder(string reminderName, TickStatus status, CancellationToken cancellationToken)
    //{
    //    if (reminderName == "firmware")
    //    {
    //        return AddInstruction("Update firmware");
    //    }
    //    return Task.CompletedTask;
    //}
}