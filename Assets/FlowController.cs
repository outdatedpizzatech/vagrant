using UnityEngine;

public class FlowController : MonoBehaviour, IObserver
{
    public MessageWindowController messageWindowController;
    public InventoryWindowController inventoryWindowController;
    public CommandWindowController commandWindowController;

    private Subject _flowSubject;
    private bool _shownInteractionMenu;
    private Encounter _queuedEncounter;
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

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case EventTopic.EndEventSequence:
                if (!_shownInteractionMenu)
                {
                    _flowSubject.Notify(FlowTopic.LoseInteractionTarget);
                }

                if (_queuedEncounter != null)
                {
                    _flowSubject.Notify(new StartEncounter(_queuedEncounter));
                    _queuedEncounter = null;
                }

                break;
            case EventTopic.ReachedEndOfMessage:
                _shownInteractionMenu = false;

                break;
            case FlowTopic.EndInteraction:
                _flowSubject.Notify(FlowTopic.CloseCommandWindow);
                _flowSubject.Notify(EventTopic.EndEventSequence);
                _flowSubject.Notify(FlowTopic.LoseInteractionTarget);

                break;
            case FlowTopic.LoseInteractionTarget:
                _interactable = null;

                break;
            case StartEncounter:
                _inEncounter = true;
                break;
            case FlowTopic.EndEncounter:
                _inEncounter = false;
                break;
            case GeneralTopic.PlayerRequestsSecondaryAction:
                if (!_inEncounter && !commandWindowController.IsVisible() && !inventoryWindowController.IsVisible() &&
                    !messageWindowController.IsVisible())
                {
                    _flowSubject.Notify(FlowTopic.OpenInventoryMenu);
                }

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

                    _flowSubject.Notify(FlowTopic.CloseInventoryMenu);
                    _flowSubject.Notify(FlowTopic.CloseCommandWindow);
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
        return _eventStepMarker.IsReadyToYieldEncounter() &&
               _queuedEncounter == null;
    }

    private void Update()
    {
        if (ShouldShowInteractionMenu())
        {
            _shownInteractionMenu = true;
            _flowSubject.Notify(FlowTopic.OpenCommandWindow);
        }
        else if (ShouldStartEncounter())
        {
            _queuedEncounter = _eventStepMarker.Encounter();
        }
    }
}