using UnityEngine;

public enum SubjectMessage
{
    PlayerRequestingWarp,
    ScreenFinishedWipeOut,
    ScreenFinishedWipeIn,
    EndDialogue,
}

public class InteractionResponseEvent
{
    public InteractionResponseEvent(string message)
    {
        Message = message;
    }

    public string Message { get; }
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
    public LeftPositionEvent(GameObject gameObject, int x, int y)
    {
        GameObject = gameObject;
        X = x;
        Y = y;
    }

    public GameObject GameObject { get; }
    public int X { get; }
    public int Y { get; }
}
