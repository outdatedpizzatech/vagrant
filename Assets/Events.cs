using System.Collections.Generic;
using UnityEngine;

public enum SubjectMessage
{
    PlayerRequestingWarp,
    ScreenFinishedWipeOut,
    ScreenFinishedWipeIn,
    AdvanceEvent,
    EndEventSequence,
    PlayerRequestsSecondaryAction,
    OpenInventoryMenu,
    CloseInventoryMenu,
    ReachedEndOfMessage,
    MenuSelection,
    TimeShouldFreeze,
    TimeShouldFlow,
    OpenInteractionMenu,
    CloseInteractionMenu,
    EndInteraction,
    GiveContextToInteractionMenu,
    StartEncounter,
    EndEncounter,
    EncounterFinishedWipeIn,
    EncounterStartWipeOut,
    PickedAttack,
    Cancel,
    OpenMainMenu
}

public class GiveItemEvent
{
    public GiveItemEvent(Item item)
    {
        Item = item;
    }

    public Item Item { get; }
}

public class ReceiveItem
{
    public ReceiveItem(Item item)
    {
        Item = item;
    }

    public Item Item { get; }
}

public class LoseItem
{
    public LoseItem(Item item)
    {
        Item = item;
    }

    public Item Item { get; }
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
    public InteractionResponseEvent(InteractionEvent interactionEvent)
    {
        InteractionEvent = interactionEvent;
    }

    public InteractionEvent InteractionEvent { get; }
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

public class PlayerRequestsPrimaryActionEvent
{
    public PlayerRequestsPrimaryActionEvent(int x, int y, Enums.Direction direction)
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

public class StartEventStep
{
    public StartEventStep(int eventStepIndex)
    {
        EventStepIndex = eventStepIndex;
    }

    public int EventStepIndex { get; }
}

public class SelectInventoryItemEvent
{
    public SelectInventoryItemEvent(Item item)
    {
        Item = item;
    }

    public Item Item { get; }
}

public class InteractWith
{
    public InteractWith(Interactable interactable, Enums.Direction direction)
    {
        Interactable = interactable;
        Direction = direction;
    }

    public Interactable Interactable { get; }
    public Enums.Direction Direction { get; }
}
