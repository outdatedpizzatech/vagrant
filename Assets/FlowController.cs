using UnityEngine;

public class FlowController : MonoBehaviour, IObserver
{
    public MessageWindowController messageWindowController;
    public InventoryWindowController inventoryWindowController;
    public CommandWindowController commandWindowController;

    private Subject _flowSubject;
    private bool _shownInteractionMenu;
    private bool _encounterIsEnqueued;
    private Interactable _interactable;
    private EventStepMarker _eventStepMarker;
    private bool _inEncounter;

    public void Setup(Subject flowSubject, Subject interactionSubject)
    {
        _flowSubject = flowSubject;
        _flowSubject.AddObserver(this);
        _eventStepMarker = new EventStepMarker(flowSubject, messageWindowController, interactionSubject);
        interactionSubject.AddObserver(this);
    }

    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.EndEventSequence:
                if (!_shownInteractionMenu)
                {
                    _flowSubject.Notify(SubjectMessage.LoseInteractionTarget);
                }

                if (_encounterIsEnqueued)
                {
                    _encounterIsEnqueued = false;
                    _flowSubject.Notify(SubjectMessage.StartEncounter);
                }

                break;
            case SubjectMessage.ReachedEndOfMessage:
                _shownInteractionMenu = false;

                break;
            case SubjectMessage.EndInteraction:
                _flowSubject.Notify(SubjectMessage.CloseInteractionMenu);
                _flowSubject.Notify(SubjectMessage.EndEventSequence);
                _flowSubject.Notify(SubjectMessage.LoseInteractionTarget);

                break;
            case SubjectMessage.LoseInteractionTarget:
                _interactable = null;

                break;
            case SubjectMessage.StartEncounter:
                _inEncounter = true;
                break;
            case SubjectMessage.EndEncounter:
                _inEncounter = false;
                break;
            case SubjectMessage.PlayerRequestsSecondaryAction:
                if (!_inEncounter && !commandWindowController.IsVisible() && !inventoryWindowController.IsVisible() &&
                    !messageWindowController.IsVisible())
                {
                    _flowSubject.Notify(SubjectMessage.OpenInventoryMenu);
                }

                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
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
                var receivedFromDirection = Utilities.GetOppositeDirection(interactWithEvent.Direction);
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

    private bool ShouldShowInteractionMenu()
    {
        return _eventStepMarker.IsReadyToYield(PostEvent.CanFollowUp) &&
               !_shownInteractionMenu;
    }

    private bool ShouldStartEncounter()
    {
        return _eventStepMarker.IsReadyToYield(PostEvent.TriggersEncounter) &&
               !_encounterIsEnqueued;
    }

    private void Update()
    {
        if (ShouldShowInteractionMenu())
        {
            _shownInteractionMenu = true;
            _flowSubject.Notify(SubjectMessage.OpenInteractionMenu);
        }
        else if (ShouldStartEncounter())
        {
            _encounterIsEnqueued = true;
        }
    }
}