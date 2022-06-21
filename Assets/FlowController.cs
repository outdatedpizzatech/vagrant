using System.Linq;
using UnityEngine;

public class FlowController : MonoBehaviour, IObserver
{
    public MessageBoxController messageBoxController;

    private Subject _flowSubject;
    private InteractionEvent _interactionEvent;
    private int _eventStepIndex;
    private bool _atEndOfMessage;
    
    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
    }
    
    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.AdvanceEvent:
                if (_atEndOfMessage)
                {
                    AdvanceEventSequenceEvent();
                }

                break;
            
            case SubjectMessage.ReachedEndOfMessageEvent:
                _atEndOfMessage = true;

                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case InteractionResponseEvent interactionResponseEvent:
                ProcessEvent(interactionResponseEvent.InteractionEvent);
                break;
        }
    }


    private void ProcessEvent(InteractionEvent interactionEvent)
    {
        _eventStepIndex = 0;
        _interactionEvent = interactionEvent;
        _atEndOfMessage = false;
        _flowSubject.Notify(new StartEventStepEvent(_eventStepIndex));
    }

    private void AdvanceEventSequenceEvent()
    {
        if (_eventStepIndex >= _interactionEvent.EventSteps.Count - 1)
        {
            var currentMessage = CurrentMessage();
            
            if (currentMessage.Information is Item item)
            {
                _flowSubject.Notify(new ReceiveItemEvent(item));
            }
            
            if (_interactionEvent.Prompts.Any())
            {
                var selectedPrompt = messageBoxController.SelectedPrompt();
                _flowSubject.Notify(
                    new PromptResponseEvent(selectedPrompt.ID));
            }
            else
            {
                _flowSubject.Notify(SubjectMessage.EndEventSequenceEvent);
            }
        }
        else
        {
            _eventStepIndex++;
            _flowSubject.Notify(new StartEventStepEvent(_eventStepIndex));
        }
    }

    private EventStep CurrentMessage()
    {
        return (_interactionEvent.EventSteps[_eventStepIndex]);
    }
}
