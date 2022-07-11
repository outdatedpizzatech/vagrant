using UnityEngine;

public class EventStepMarker : MonoBehaviour
{
    private int _eventStepIndex;
    private InteractionEvent _interactionEvent;
    private bool _atEndOfMessage;

    public void StartNew(InteractionEvent interactionEvent)
    {
        _eventStepIndex = 0;
        _interactionEvent = interactionEvent;
        _atEndOfMessage = false;
    }

    public void StartNextEventStep()
    {
        _eventStepIndex++;
        _atEndOfMessage = false;
    }

    public void End()
    {
        _interactionEvent = null;
    }

    public bool IsAtEndOfMessage()
    {
        return _atEndOfMessage;
    }

    public void ReachedEndOfMessage()
    {
        _atEndOfMessage = true;
    }

    public int EventStepIndex()
    {
        return _eventStepIndex;
    }

    public InteractionEvent InteractionEvent()
    {
        return _interactionEvent;
    }

    public EventStep CurrentMessage()
    {
        return _interactionEvent.EventSteps[_eventStepIndex];
    }

    public bool AtEndOfEvent()
    {
        return _eventStepIndex >= _interactionEvent.EventSteps.Count - 1;
    }

    public bool IsReadyToYield(PostEvent postEvent)

    {
        return _interactionEvent != null && _atEndOfMessage && AtEndOfEvent() &&
               _interactionEvent.Information is PostEvent informationPostEvent && informationPostEvent == postEvent;
    }
}