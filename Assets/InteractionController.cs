using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour, IObserver
{
    public ContextController contextController;

    private bool _inMenu;
    private bool _inEvent;
    private bool _halted;
    private Subject _interactionSubject;
    private Subject _flowSubject;
    private PositionGrid _positionGrid;
    private InputAction _inputAction;
    private float _timeSinceLastDirectionalInput;
    private float _timeSinceLastSecondaryAction;

    private void Update()
    {
        _timeSinceLastDirectionalInput += Time.deltaTime;
        _timeSinceLastSecondaryAction += Time.deltaTime;

        if (contextController.activeContexts.Any((x) => x == ContextController.ControlContext.FollowUpMenu || x == ContextController.ControlContext.InventoryMenu))
        {
            _inMenu = true;
        }
        else
        {
            _inMenu = false;
        }
        if (contextController.activeContexts.Any((x) => x == ContextController.ControlContext.Event))
        {
            _inEvent = true;
        }
        else
        {
            _inEvent = false;
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
                if (_currentControlContext() == ContextController.ControlContext.FollowUpMenu)
                {
                    _flowSubject.Notify(new FollowUpMenuNavigation(_inputAction.InputDirections.Last()));
                }
                else
                {
                    _flowSubject.Notify(new MenuNavigation(_inputAction.InputDirections.Last()));
                }
            }
        }

        Utilities.Debounce(ref _timeSinceLastDirectionalInput, 0.25f, NotifyMenuInputs);
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
        
        contextController.Setup(flowSubject);
    }

    public void OnNotify(SubjectMessage subjectMessage)
    {
        switch (subjectMessage)
        {
            case SubjectMessage.EndEventSequenceEvent:
                _inEvent = false;
                break;
            case SubjectMessage.RequestFollowUpEvent:
                _inMenu = true;
                break;
            case SubjectMessage.EndFollowUpEvent:
                _inMenu = false;
                break;
            case SubjectMessage.OpenMenuEvent:
                break;
            case SubjectMessage.PlayerRequestsSecondaryActionEvent:
                if (!_inEvent && !_inMenu)
                {
                    _flowSubject.Notify(SubjectMessage.OpenMenuEvent);
                    _inMenu = true;
                }
                else if (_inMenu)
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
                    var interactable = _positionGrid.Get(position[0], position[1]).GetComponent<IInteractable>();

                    if (interactable != null)
                    {
                        var interactWithEvent = new InteractWithEvent(interactable, playerActionEvent.Direction);
                        _flowSubject.Notify(interactWithEvent);
                    }
                }

                break;
            }
            case PlayerRequestsPrimaryActionEvent:
            {
                if (_currentControlContext() == ContextController.ControlContext.Event)
                {
                    _flowSubject.Notify(SubjectMessage.AdvanceEvent);
                }
                else if (_currentControlContext() == ContextController.ControlContext.FollowUpMenu)
                {
                    _flowSubject.Notify(SubjectMessage.SelectFollowUpMenuItem);
                }
                else if (_currentControlContext() == ContextController.ControlContext.InventoryMenu)
                {
                    _flowSubject.Notify(SubjectMessage.SelectInventoryMenuItem);
                }

                break;
            }
        }
    }

    private ContextController.ControlContext _currentControlContext()
    {
        if (contextController.activeContexts.Count == 0)
        {
            return ContextController.ControlContext.None;
        }

        return (contextController.activeContexts.Last());
    }
}