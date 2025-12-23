using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansBook.GrainClasses;

public class EventSourcedState
{
    Queue<string> instructions = new Queue<string>();
    public int Count => instructions.Count;
    public void Apply(EnqueueEvent @event) => instructions.Enqueue(@event.Value);
    public void Apply(DequeueEvent @event)
    {
        if (instructions.Count == 0) return;
        @event.Value = instructions.Dequeue();
    }
}
