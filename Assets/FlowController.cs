using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowController : MonoBehaviour, IObserver
{
    public MessageBoxController messageBoxController;

    private Subject _flowSubject;
    private bool _shownInteractionMenu;
    private bool _encounterIsEqueued;
    private Interactable _interactable;
    private readonly EventStepMarker _eventStepMarker = new();

    public void Update()
    {
        if (ShouldShowInteractionMenu())
        {
            _shownInteractionMenu = true;
            _flowSubject.Notify(SubjectMessage.OpenInteractionMenu);
        }
        else if (ShouldStartEncounter())
        {
            _encounterIsEqueued = true;
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
            case SubjectMessage.EndEventSequence:
                _interactable = null;
                _eventStepMarker.End();

                if (_encounterIsEqueued)
                {
                    _encounterIsEqueued = false;
                    _flowSubject.Notify(SubjectMessage.StartEncounter);
                }

                break;
            case SubjectMessage.AdvanceEvent:
                if (_eventStepMarker.IsAtEndOfMessage())
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

                _eventStepMarker.ReachedEndOfMessage();
                _shownInteractionMenu = false;

                break;

            case SubjectMessage.EndInteraction:
                _flowSubject.Notify(SubjectMessage.CloseInteractionMenu);
                _flowSubject.Notify(SubjectMessage.EndEventSequence);

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
        _eventStepMarker.StartNew(interactionEvent, _flowSubject);
    }

    private void AdvanceEventSequence()
    {
        if (AtEndOfEvent())
        {
            if (_eventStepMarker.InteractionEvent().Information is List<Prompt> prompts && prompts.Any())
            {
                var selectedPrompt = messageBoxController.SelectedPrompt();
                _flowSubject.Notify(
                    new PromptResponseEvent(selectedPrompt.ID));
                return;
            }

            _flowSubject.Notify(SubjectMessage.EndEventSequence);
        }
        else
        {
            _eventStepMarker.StartNextEventStep();
            _flowSubject.Notify(new StartEventStep(_eventStepMarker.EventStepIndex()));
        }
    }

    private EventStep CurrentMessage()
    {
        return _eventStepMarker.CurrentMessage();
    }

    private bool ShouldShowInteractionMenu()
    {
        return _eventStepMarker.IsReadyToYield(PostEvent.CanFollowUp) &&
               !_shownInteractionMenu;
    }

    private bool ShouldStartEncounter()
    {
        return _eventStepMarker.IsReadyToYield(PostEvent.TriggersEncounter) &&
               !_encounterIsEqueued;
    }

    private bool AtEndOfEvent()
    {
        return _eventStepMarker.AtEndOfEvent();
    }
}