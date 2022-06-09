using System.Collections.Generic;
using UnityEngine;

public enum SubjectMessage
{
    PlayerRequestingWarp,
    ScreenFinishedWipeOut,
    ScreenFinishedWipeIn,
    AdvanceDialogue,
    EndDialogue,
    StartDialogue,
}

public class PromptResponseEvent
{
    public PromptResponseEvent(object promptResponse)
    {
        PromptResponse = promptResponse;
    }

    public object PromptResponse { get; }
}

public class InteractionResponseEvent
{
    public InteractionResponseEvent(List<MessageEnvelope> responses)
    {
        Responses = responses;
    }

    public List<MessageEnvelope> Responses { get; }
}

public class MenuNavigation
{
    public MenuNavigation(Enums.Direction direction)
    {
        Direction = direction;
    }

    public Enums.Direction Direction { get; }
}

public class PlayerBeganWarpingEvent
{
    public PlayerBeganWarpingEvent(Enums.Direction facingDirection, int x, int y)
    {
        FacingDirection = facingDirection;
        X = x;
        Y = y;
    }

    public Enums.Direction FacingDirection { get; }
    public int X { get; }
    public int Y { get; }
}

public class EnteredPositionEvent
{
    public EnteredPositionEvent(GameObject gameObject, int x, int y)
    {
        GameObject = gameObject;
        X = x;
        Y = y;
    }

    public GameObject GameObject { get; }
    public int X { get; }
    public int Y { get; }
}

public class PlayerActionEvent
{
    public PlayerActionEvent(int x, int y, Enums.Direction direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }

    public int X { get; }
    public int Y { get; }
    public Enums.Direction Direction { get; }
}

public class LeftPositionEvent
{
    public LeftPositionEvent(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
}
