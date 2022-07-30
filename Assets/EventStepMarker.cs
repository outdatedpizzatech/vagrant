using System.Collections.Generic;
using System.Linq;

public class EventStepMarker : IObserver
{
    private int _eventStepIndex;
    private InteractionEvent _interactionEvent;
    private bool _atEndOfMessage;

    private readonly Subject _subject;
    private readonly MessageWindowController _messageWindowController;

    public EventStepMarker(Subject subject, MessageWindowController messageWindowController, Subject interactionSubject)
    {
        _subject = subject;
        subject.AddObserver(this);
        _messageWindowController = messageWindowController;
        interactionSubject.AddObserver(this);
    }

    public bool Active()
    {
        return (_interactionEvent != null);
    }

    public bool IsReadyToYield(PostEvent postEvent)

    {
        return _interactionEvent != null && _atEndOfMessage && AtEndOfEvent() &&
               _interactionEvent.Information is PostEvent informationPostEvent && informationPostEvent == postEvent;
    }

    public bool IsReadyToYieldEncounter()
    {
        return _interactionEvent != null && _atEndOfMessage && AtEndOfEvent() &&
               _interactionEvent.Information is Encounter;
    }

    public Encounter Encounter()
    {
        if (_interactionEvent.Information is Encounter encounter)
        {
            return encounter;
        }

        return null;
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case InteractionResponseEvent interactionResponseEvent:
                StartNew(interactionResponseEvent.InteractionEvent);
                break;
            case EventTopic.ReachedEndOfMessage:
                _atEndOfMessage = true;

                var currentMessage = CurrentMessage();

                if (currentMessage.Information is Item item)
                {
                    _subject.Notify(new ReceiveItem(item));
                }

                break;
            case EventTopic.EndEventSequence:
                _interactionEvent = null;
                break;
            case EventTopic.AdvanceEvent when _atEndOfMessage:
                AdvanceEventSequence();
                break;
            case PlayerRequestsPrimaryActionEvent when _messageWindowController.IsFocused():
                _subject.Notify(EventTopic.AdvanceEvent);
                break;
        }
    }

    private void StartNew(InteractionEvent interactionEvent)
    {
        _eventStepIndex = 0;
        _interactionEvent = interactionEvent;
        _atEndOfMessage = false;
        _subject.Notify(new StartEventStep(_eventStepIndex));
    }

    private void AdvanceEventSequence()
    {
        if (AtEndOfEvent())
        {
            if (_interactionEvent.Information is List<Prompt> prompts && prompts.Any())
            {
                var selectedPrompt = _messageWindowController.SelectedPrompt();
                _subject.Notify(
                    new PromptResponseEvent(selectedPrompt.ID));
                return;
            }

            _subject.Notify(EventTopic.EndEventSequence);
        }
        else
        {
            StartNextEventStep();
        }
    }

    private EventStep CurrentMessage()
    {
        return _interactionEvent.EventSteps[_eventStepIndex];
    }

    private void StartNextEventStep()
    {
        _eventStepIndex++;
        _atEndOfMessage = false;
        _subject.Notify(new StartEventStep(_eventStepIndex));
    }

    private bool AtEndOfEvent()
    {
        return _eventStepIndex >= _interactionEvent.EventSteps.Count - 1;
    }
}