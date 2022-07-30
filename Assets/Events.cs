using UnityEngine;

public enum GeneralTopic
{
    PlayerRequestingWarp,
    ScreenFinishedWipeOut,
    ScreenFinishedWipeIn,
    PlayerRequestsSecondaryAction,
}

public enum EventTopic
{
    ReachedEndOfMessage,
    AdvanceEvent,
    EndEventSequence,
}

public enum FlowTopic
{
    LoseInteractionTarget,
    EndEncounter,
    EncounterFinishedWipeIn,
    EncounterStartWipeOut,
    EndInteraction,
    CloseCommandWindow,
    OpenCommandWindow,
    TimeShouldFreeze,
    TimeShouldFlow,
    OpenInventoryMenu,
    CloseInventoryMenu,
}

public enum EncounterTopic
{
    AttemptingToFlee,
    EndAttackAnimation,
    CloseMainMenu,
    Cancel,
    EndDamageAnimation,
}

public class PickedAbility
{
    public PickedAbility(Ability ability)
    {
        Ability = ability;
    }

    public Ability Ability { get; }
}

public class OpenEncounterCommandWindow
{
    public OpenEncounterCommandWindow(Damageable damageable)
    {
        Damageable = damageable;
    }

    public Damageable Damageable { get; }
}

public class GainFocus
{
    public GainFocus(Window window)
    {
        Window = window;
    }

    public Window Window { get; }
}

public class DismissWindow
{
    public DismissWindow(Window window)
    {
        Window = window;
    }

    public Window Window { get; }
}

public class RegainFocus
{
    public RegainFocus(Window window)
    {
        Window = window;
    }

    public Window Window { get; }
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

public class DirectionalNavigation
{
    public DirectionalNavigation(Enums.Direction direction)
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
    public LeftPositionEvent(int x, int y, bool isPlayer)
    {
        X = x;
        Y = y;
        IsPlayer = isPlayer;
    }

    public int X { get; }
    public int Y { get; }
    public bool IsPlayer { get; }
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

public class StartEncounter
{
    public StartEncounter(Encounter encounter)
    {
        Encounter = encounter;
    }
    
    public Encounter Encounter;
}

namespace EncounterEvents
{
    public class EncounterMessage
    {
        public EncounterMessage(string message)
        {
            Message = message;
        }

        public string Message;
    }

    public class BeginAction
    {
        public BeginAction(Damageable actor, Ability ability)
        {
            Actor = actor;
            Ability = ability;
        }

        public Damageable Actor;
        public Ability Ability;
    }
}