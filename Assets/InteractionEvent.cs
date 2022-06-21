using System.Collections.Generic;

public class EventStep
{
    public EventStep(object information)
    {
        Information = information;
    }
    
    public object Information { get; }
}

public class InteractionEvent
{
    public readonly List<EventStep> EventSteps = new();
    public readonly List<Prompt> Prompts = new();

    public void AddMessage(string message)
    {
        EventSteps.Add(new EventStep(message));
    }

    public void AddItem(Item item)
    {
        EventSteps.Add(new EventStep(item));
    }
}