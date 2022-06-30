using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour, IObserver
{
    private bool _inMenu;
    private bool _inEvent;
    private bool _halted;
    private Subject _interactionSubject;
    private Subject _flowSubject;
    private PositionGrid _positionGrid;
    private InputAction _inputAction;
    private float _timeSinceLastDirectionalInput;
    private float _timeSinceLastSecondaryAction;
    private IInteractable _interactable;
    private ControlContext currentControlContext;

    private enum ControlContext
    {
        Event,
        Menu,
        None
    }

    private void Update()
    {
        _timeSinceLastDirectionalInput += Time.deltaTime;
        _timeSinceLastSecondaryAction += Time.deltaTime;

        if (!_inEvent && !_inMenu)
        {
            currentControlContext = ControlContext.None;
        }

        if (!_halted)
        {
            if (_inMenu || _inEvent)
            {
                _flowSubject.Notify(SubjectMessage.StartHaltedContextEvent);
                _halted = true;
            }
        }
        else
        {
            if (!_inMenu && !_inEvent)
            {
                _flowSubject.Notify(SubjectMessage.EndHaltedContextEvent);
                _halted = false;
            }
        }

        void NotifyMenuInputs()
        {
            if ((_inEvent || _inMenu) && _inputAction.InputDirections.Any())
            {
                _flowSubject.Notify(new MenuNavigation(_inputAction.InputDirections.Last()));
            }
        }

        Utilities.Debounce(ref _timeSinceLastDirectionalInput, 0.05f, NotifyMenuInputs);
    }

    public void Setup(Subject interactionSubject, PositionGrid positionGrid, Subject flowSubject,
        InputAction inputAction)
    {
        _interactionSubject = interactionSubject;
        _positionGrid = positionGrid;
        _flowSubject = flowSubject;
        _inputAction = inputAction;

        _interactionSubject.AddObserver(this);
        _flowSubject.AddObserver(this);
    }

    public void OnNotify(SubjectMessage subjectMessage)
    {
        switch (subjectMessage)
        {
            case SubjectMessage.EndEventSequenceEvent:
                _interactable = null;
                _inEvent = false;
                break;
            case SubjectMessage.RequestFollowUpEvent:
                _inMenu = true;
                currentControlContext = ControlContext.Menu;
                break;
            case SubjectMessage.EndFollowUpEvent:
                _inMenu = false;
                break;
            case SubjectMessage.PlayerRequestsSecondaryActionEvent:
                if (!_inEvent && !_inMenu)
                {
                    _flowSubject.Notify(SubjectMessage.OpenMenuEvent);
                    _inMenu = true;
                    currentControlContext = ControlContext.Menu;

                } else if (_inMenu)
                {
                    _inMenu = false;
                    _flowSubject.Notify(SubjectMessage.CloseMenuEvent);
                }

                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case PlayerRequestsPrimaryActionEvent playerActionEvent when !_inEvent && !_inMenu:
            {
                var position = new[] { playerActionEvent.X, playerActionEvent.Y };

                switch (playerActionEvent.Direction)
                {
                    case Enums.Direction.Down:
                        position[1] -= 1;
                        break;
                    case Enums.Direction.Up:
                        position[1] += 1;
                        break;
                    case Enums.Direction.Left:
                        position[0] -= 1;
                        break;
                    case Enums.Direction.Right:
                        position[0] += 1;
                        break;
                }

                if (_positionGrid.Has(position[0], position[1]))
                {
                    _interactable = _positionGrid.Get(position[0], position[1]).GetComponent<IInteractable>();

                    if (_interactable != null)
                    {
                        var receivedFromDirection = (Enums.Direction)(((int)playerActionEvent.Direction + 2) % 4);
                        var interactionResponse = _interactable.ReceiveInteraction(receivedFromDirection);
                        if (interactionResponse != null)
                        {
                            _flowSubject.Notify(new InteractionResponseEvent(interactionResponse));
                            _flowSubject.Notify(SubjectMessage.StartEventSequenceEvent);
                        }
                    }
                }

                break;
            }
            case PlayerRequestsPrimaryActionEvent:
            {
                if (currentControlContext == ControlContext.Event)
                {
                    _flowSubject.Notify(SubjectMessage.AdvanceEvent);
                } else if(currentControlContext == ControlContext.Menu)
                {
                    _flowSubject.Notify(SubjectMessage.SelectMenuItem);
                }

                break;
            }
            case InteractionResponseEvent:
                _inEvent = true;
                currentControlContext = ControlContext.Event;
                break;
            case PromptResponseEvent promptResponseEvent:
                _flowSubject.Notify(
                    new InteractionResponseEvent(_interactable.ReceiveInteraction(promptResponseEvent.PromptResponse)));
                break;
        }
    }
}