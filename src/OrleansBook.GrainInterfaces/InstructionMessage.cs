using Orleans;

namespace OrleansBook.GrainInterfaces;

[GenerateSerializer]
public class InstructionMessage
{
    public InstructionMessage()
    {
    }

    public InstructionMessage(string instruction, string robot)
    {
        this.Instruction = instruction;
        this.Robot = robot;
    }

    [Id(0)]
    public string Instruction { get; set; } = string.Empty;
    [Id(1)]
    public string Robot { get; set; } = string.Empty;
}