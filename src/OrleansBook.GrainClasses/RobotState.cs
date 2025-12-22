namespace OrleansBook.GrainClasses;

public class RobotState
{
    public Queue<string> Instructions { get; } = new Queue<string>();
}