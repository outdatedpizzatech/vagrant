public class EventStepMarker : IObserver
{
    private int _eventStepIndex;
    private InteractionEvent _interactionEvent;
    private bool _atEndOfMessage;

    private readonly Subject _subject;

    public EventStepMarker(Subject subject)
    {
        _subject = subject;
        subject.AddObserver(this);
    }

    private void StartNew(InteractionEvent interactionEvent)
    {
        _eventStepIndex = 0;
        _interactionEvent = interactionEvent;
        _atEndOfMessage = false;
        _subject.Notify(new StartEventStep(EventStepIndex()));
    }

    public void StartNextEventStep()
    {
        _eventStepIndex++;
        _atEndOfMessage = false;
        _subject.Notify(new StartEventStep(EventStepIndex()));
    }

    public void End()
    {
        _interactionEvent = null;
    }

    public bool IsAtEndOfMessage()
    {
        return _atEndOfMessage;
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

    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.ReachedEndOfMessage:
                ReachedEndOfMessage();
                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case InteractionResponseEvent interactionResponseEvent:
                StartNew(interactionResponseEvent.InteractionEvent);
                break;
        }
    }

    private void ReachedEndOfMessage()
    {
        _atEndOfMessage = true;
    }

    private int EventStepIndex()
    {
        return _eventStepIndex;
    }
}