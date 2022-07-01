using System.Linq;
using UnityEngine;

public class FlowController : MonoBehaviour, IObserver
{
    public MessageBoxController messageBoxController;

    private Subject _flowSubject;
    private InteractionEvent _interactionEvent;
    private int _eventStepIndex;
    private bool _atEndOfMessage;
    private bool _requestedFollowUp;
    private IInteractable _interactable;

    public void Update()
    {
        if (_interactionEvent != null && !_requestedFollowUp && _atEndOfMessage && _eventStepIndex >= _interactionEvent.EventSteps.Count - 1 && !_interactionEvent.Prompts.Any() && _interactionEvent.CanFollowUp)
        {
            _requestedFollowUp = true;
            _flowSubject.Notify(SubjectMessage.RequestFollowUpEvent);
        }
    }
    
    public void Setup(Subject flowSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
    }
    
    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.EndEventSequenceEvent:
                _interactable = null;
                break;
            case SubjectMessage.AdvanceEvent:
                if (_atEndOfMessage)
                {
                    AdvanceEventSequenceEvent();
                }

                break;
            
            case SubjectMessage.ReachedEndOfMessageEvent:
                _atEndOfMessage = true;
                _requestedFollowUp = false;

                break;
            
            case SubjectMessage.EndFollowUpEvent:
                _flowSubject.Notify(SubjectMessage.EndEventSequenceEvent);
                _interactionEvent = null;

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
            
            case SelectInventoryItemEvent selectInventoryItemEvent:
                if (_interactable == null)
                {
                    var response = new InteractionEvent();
                    response.AddMessage(selectInventoryItemEvent.Item.description);
                    _flowSubject.Notify(new InteractionResponseEvent(response));
                    _flowSubject.Notify(SubjectMessage.StartEventSequenceEvent);
                }
                else
                {
                    var interactionResponse1 = _interactable.ReceiveInteraction(selectInventoryItemEvent.Item);
                    if (interactionResponse1 != null)
                    {
                        _flowSubject.Notify(new InteractionResponseEvent(interactionResponse1));
                    }
                    _flowSubject.Notify(SubjectMessage.CloseMenuEvent);
                }

                break;
            
            case InteractWithEvent interactWithEvent:
                _interactable = interactWithEvent.Interactable;
                int newDirection = ((int)interactWithEvent.Direction + 2) % 4;
                Enums.Direction receivedFromDirection = (Enums.Direction)newDirection;
                var interactionResponse = _interactable.ReceiveInteraction(receivedFromDirection);
                if (interactionResponse != null)
                {
                    _flowSubject.Notify(new InteractionResponseEvent(interactionResponse));
                    _flowSubject.Notify(SubjectMessage.StartEventSequenceEvent);
                }

                break;
            
            case PromptResponseEvent promptResponseEvent:
                _flowSubject.Notify(
                    new InteractionResponseEvent(_interactable.ReceiveInteraction(promptResponseEvent.PromptResponse)));
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
                if (!_interactionEvent.CanFollowUp)
                {
                    _flowSubject.Notify(SubjectMessage.EndEventSequenceEvent);
                }
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
