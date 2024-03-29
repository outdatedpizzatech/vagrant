using System.Collections.Generic;
using UnityEngine;

public enum PostEvent
{
    CanFollowUp,
}

public class Encounter
{
    public List<UnityEngine.Object> Opponents = new();

    public void Add(EncounterLoader.Encounterable encounterable)
    {
        Opponents.Add(EncounterLoader.Get(encounterable));
    }
}

public class Message
{
    public Message(string content, AnimationClip speakingAnimation, AnimationClip idleAnimation)
    {
        Content = content;
        SpeakingAnimation = speakingAnimation;
        IdleAnimation = idleAnimation;
    }

    public Message(string content)
    {
        Content = content;
    }

    public string Content { get; }
    public AnimationClip SpeakingAnimation { get; }
    public AnimationClip IdleAnimation { get; }
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
    public object Information { get; }

    public InteractionEvent()
    {
    }

    public InteractionEvent(Encounter encounter)
    {
        Information = encounter;
    }

    public InteractionEvent(List<Prompt> prompts)
    {
        Information = prompts;
    }

    public InteractionEvent(PostEvent postEvent)
    {
        Information = postEvent;
    }

    public void AddMessage(string message)
    {
        EventSteps.Add(new EventStep(new Message(message)));
    }

    public void AddMessage(string message, AnimationClip speakingAnimation, AnimationClip idleAnimation)
    {
        EventSteps.Add(new EventStep(new Message(message, speakingAnimation, idleAnimation)));
    }

    public void AddItem(Item item)
    {
        EventSteps.Add(new EventStep(item));
    }
}