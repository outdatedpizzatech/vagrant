using System.Linq;
using UnityEngine;

public class FlowController : MonoBehaviour, IObserver
{
    public MessageBoxController messageBoxController;

    private Subject _flowSubject;
    private InteractionEvent _interactionEvent;
    private int _eventStepIndex;
    private bool _atEndOfMessage;
    private bool _shownInteractionMenu;
    private Interactable _interactable;

    public void Update()
    {
        if (!ShouldShowInteractionMenu()) return;
        _shownInteractionMenu = true;
        _flowSubject.Notify(SubjectMessage.OpenInteractionMenu);
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
            case SubjectMessage.EndEventSequence:
                _interactable = null;
                break;
            case SubjectMessage.AdvanceEvent:
                if (_atEndOfMessage)
                {
                    AdvanceEventSequence();
                }

                break;
            case SubjectMessage.ReachedEndOfMessage:
                var currentMessage = CurrentMessage();

                if (currentMessage.Information is Item item)
                {
                    _flowSubject.Notify(new ReceiveItem(item));
                }

                _atEndOfMessage = true;
                _shownInteractionMenu = false;

                break;

            case SubjectMessage.EndInteraction:
                _flowSubject.Notify(SubjectMessage.CloseInteractionMenu);
                _flowSubject.Notify(SubjectMessage.EndEventSequence);

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
            {
                if (_interactable == null)
                {
                    var response = new InteractionEvent();
                    response.AddMessage(selectInventoryItemEvent.Item.description);
                    _flowSubject.Notify(new InteractionResponseEvent(response));
                }
                else
                {
                    var interactionResponse = _interactable.ReceiveItem(selectInventoryItemEvent.Item);
                    if (interactionResponse != null)
                    {
                        _flowSubject.Notify(new LoseItem(selectInventoryItemEvent.Item));
                        _flowSubject.Notify(new InteractionResponseEvent(interactionResponse));
                    }

                    _flowSubject.Notify(SubjectMessage.CloseInventoryMenu);
                    _flowSubject.Notify(SubjectMessage.CloseInteractionMenu);
                }

                break;
            }

            case InteractWith interactWithEvent:
            {
                _interactable = interactWithEvent.Interactable;
                var newDirection = ((int)interactWithEvent.Direction + 2) % 4;
                Enums.Direction receivedFromDirection = (Enums.Direction)newDirection;
                var interactionResponse = _interactable.ReceiveInteraction(receivedFromDirection);
                if (interactionResponse != null)
                {
                    _flowSubject.Notify(new InteractionResponseEvent(interactionResponse));
                }

                break;
            }

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
        _flowSubject.Notify(new StartEventStep(_eventStepIndex));
    }

    private void AdvanceEventSequence()
    {
        if (AtEndOfEvent())
        {
            if (_interactionEvent.Prompts.Any())
            {
                var selectedPrompt = messageBoxController.SelectedPrompt();
                _flowSubject.Notify(
                    new PromptResponseEvent(selectedPrompt.ID));
            }
            else
            {
                _flowSubject.Notify(SubjectMessage.EndEventSequence);
            }
        }
        else
        {
            _eventStepIndex++;
            _atEndOfMessage = false;
            _flowSubject.Notify(new StartEventStep(_eventStepIndex));
        }
    }

    private EventStep CurrentMessage()
    {
        return (_interactionEvent.EventSteps[_eventStepIndex]);
    }

    private bool ShouldShowInteractionMenu()
    {
        return _interactionEvent != null && !_shownInteractionMenu && _atEndOfMessage &&
               AtEndOfEvent() && !_interactionEvent.Prompts.Any() && _interactionEvent.CanFollowUp;
    }

    private bool AtEndOfEvent()
    {
        return _eventStepIndex >= _interactionEvent.EventSteps.Count - 1;
    }
}