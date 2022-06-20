using System.Collections.Generic;

public class EventStep
{
    public enum Types
    {
        Message = 0,
        ItemExchange = 1,
    }

    public EventStep(Types type, string message)
    {
        Type = type;
        Message = message;
    }
    
    public Types Type { get; }
    public string Message { get; }
}

public class InteractionEvent
{
    public readonly List<EventStep> EventSteps = new();
    public readonly List<Prompt> Prompts = new();
    
    /*
     * While this seems like a MessageEnvelope could receive any of these items, there are business rules which
     * invalidate certain members. For example, a prompt is only used for the last MessageEnvelope in a list
     *
     * This likely means that the domain hierarchy of MessageEnvelope is wrong and should change
     */

    public void AddMessage(string message)
    {
        EventSteps.Add(new EventStep(EventStep.Types.Message, message));
    }

    public void AddItem(string message)
    {
        EventSteps.Add(new EventStep(EventStep.Types.ItemExchange, message));
    }
}