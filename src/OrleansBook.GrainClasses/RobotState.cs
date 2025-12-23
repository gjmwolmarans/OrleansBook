namespace OrleansBook.GrainClasses;

[GenerateSerializer]
public class RobotState
{
    [Id(0)]
    public Queue<string> Instructions { get; } = new Queue<string>();
}