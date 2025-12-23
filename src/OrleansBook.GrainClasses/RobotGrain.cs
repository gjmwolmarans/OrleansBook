using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Reminders;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses;

public class RobotGrain : Grain, IRobotGrain
{
    int instructionsEnqueued = 0;
    int instructionsDequeued = 0;
    private readonly ILogger<RobotGrain> _logger;
    private readonly IPersistentState<RobotState> _state;
    private IAsyncStream<InstructionMessage> _stream;

    public RobotGrain(ILogger<RobotGrain> logger,
        [PersistentState("robotState", "robotStateStore")] IPersistentState<RobotState> state)
    {
        _logger = logger;
        _state = state;
        _stream = this.GetStreamProvider("StreamProvider")
            .GetStream<InstructionMessage>("StartingInstruction", Guid.Empty);
    }

    async Task Publish(string instruction)
    {
        var key = this.GetPrimaryKeyString();
        var message = new InstructionMessage(instruction, key);
        await _stream.OnNextAsync(message);
    }

    public async Task AddInstruction(string instruction)
    {
        var key = this.GetPrimaryKeyString();
        _logger.LogWarning("{Key} adding '{Instruction}'", key, instruction);
        _state.State.Instructions.Enqueue(instruction);
        instructionsEnqueued += 1;
        await _state.WriteStateAsync();
    }

    public Task<int> GetInstructionCount()
    {
        return Task.FromResult(_state.State.Instructions.Count);
    }

    public async Task<string> GetNextInstruction()
    {
        if (_state.State.Instructions.Count == 0)
        {
            return null;
        }
        var instruction = _state.State.Instructions.Dequeue();
        var key = this.GetPrimaryKeyString();
        _logger.LogWarning("{Key} adding '{Instruction}'", key, instruction);
        await Publish(instruction);
        instructionsDequeued += 1;
        await _state.WriteStateAsync();
        return instruction;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var oneMinute = TimeSpan.FromMinutes(1);
        this.RegisterGrainTimer<object>(this.ResetsStats, null, oneMinute, oneMinute);
        var oneDay = TimeSpan.FromDays(1);
        this.RegisterOrUpdateReminder("firmware", oneDay, oneDay);
        return base.OnActivateAsync(cancellationToken);
    }

    private Task ResetsStats(object _, CancellationToken cancellationToken)
    {
        var key = this.GetPrimaryKeyString();
        Console.WriteLine($"{key} enqueued: {instructionsEnqueued}");
        Console.WriteLine($"{key} dequeued: {instructionsDequeued}");
        Console.WriteLine($"{key} queued: {_state.State.Instructions.Count}");
        instructionsEnqueued = 0;
        instructionsDequeued = 0;
        return Task.CompletedTask;
    }

    public Task ReceiveReminder(string reminderName, TickStatus status, CancellationToken cancellationToken)
    {
        if (reminderName == "firmware")
        {
            return AddInstruction("Update firmware");
        }
        return Task.CompletedTask;
    }
}