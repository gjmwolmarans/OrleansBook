using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses;

public class RobotGrain : Grain, IRobotGrain
{
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
        await _state.WriteStateAsync();
        return instruction;
    }
}