using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public Message(string content, AnimationClip expression)
    {
        Content = content;
        Expression = expression;
    }

    public Message(string content)
    {
        Content = content;
    }

    public string Content { get; }
    public AnimationClip Expression { get; }
}

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
    public bool CanFollowUp = false;

    public void AddMessage(string message)
    {
        EventSteps.Add(new EventStep(new Message(message)));
    }

    public void AddMessage(string message, AnimationClip expression)
    {
        EventSteps.Add(new EventStep(new Message(message, expression)));
    }

    public void AddItem(Item item)
    {
        EventSteps.Add(new EventStep(item));
    }
}