using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour, IObserver
{
    public ContextController contextController;
    public float menuDirectionalDebounceTiming;

    private bool _inEvent;
    private bool _inEncounter;
    private bool _aMenuIsOpen;
    private bool _halted;
    private Subject _interactionSubject;
    private Subject _flowSubject;
    private Subject _encounterSubject;
    private PositionGrid _positionGrid;
    private InputAction _inputAction;
    private float _timeSinceLastDirectionalInput;
    private bool _inventoryMenuFocused;
    private bool _messageBoxFocused;
    private bool _interactionMenuFocused;
    private bool _requestedInteractionmenuFocus;

    private void Update()
    {
        _timeSinceLastDirectionalInput += Time.deltaTime;

        var currentControlContext = contextController.Current();

        _interactionMenuFocused = currentControlContext == Enums.ControlContext.InteractionMenu;
        _inventoryMenuFocused = currentControlContext == Enums.ControlContext.InventoryMenu;
        _messageBoxFocused = currentControlContext == Enums.ControlContext.Event;

        var interactionMenuOpen =
            _interactionMenuFocused || contextController.InHistory(Enums.ControlContext.InteractionMenu);
        var inventoryMenuOpen =
            _inventoryMenuFocused || contextController.InHistory(Enums.ControlContext.InventoryMenu);

        _aMenuIsOpen = interactionMenuOpen || inventoryMenuOpen;
        _inEvent = _messageBoxFocused || contextController.InHistory(Enums.ControlContext.Event);

        if (!_halted)
        {
            if (!IsFreeRoaming())
            {
                _flowSubject.Notify(SubjectMessage.TimeShouldFreeze);
                _halted = true;
            }
        }
        else
        {
            if (IsFreeRoaming())
            {
                _flowSubject.Notify(SubjectMessage.TimeShouldFlow);
                _halted = false;
            }
        }

        if (_interactionMenuFocused && !_requestedInteractionmenuFocus)
        {
            _requestedInteractionmenuFocus = true;
            _flowSubject.Notify(SubjectMessage.GiveContextToInteractionMenu);
        }
        else if (!_interactionMenuFocused)
        {
            _requestedInteractionmenuFocus = false;
        }

        void NotifyMenuInputs()
        {
            if (IsFreeRoaming() || !_inputAction.InputDirections.Any()) return;
            if (_inEncounter)
            {
                _encounterSubject.Notify(new MenuNavigation(_inputAction.InputDirections.Last()));
            }
            else
            {
                _flowSubject.Notify(new MenuNavigation(_inputAction.InputDirections.Last()));
            }
        }

        Utilities.Debounce(ref _timeSinceLastDirectionalInput, menuDirectionalDebounceTiming, NotifyMenuInputs);
    }

    public void Setup(Subject interactionSubject, PositionGrid positionGrid, Subject flowSubject,
        InputAction inputAction, Subject encounterSubject)
    {
        _interactionSubject = interactionSubject;
        _positionGrid = positionGrid;
        _flowSubject = flowSubject;
        _encounterSubject = encounterSubject;
        _inputAction = inputAction;

        _interactionSubject.AddObserver(this);
        _flowSubject.AddObserver(this);

        contextController.Setup(flowSubject);
    }

    public void OnNotify(SubjectMessage subjectMessage)
    {
        switch (subjectMessage)
        {
            case SubjectMessage.PlayerRequestsSecondaryAction:
                if (IsFreeRoaming())
                {
                    _flowSubject.Notify(SubjectMessage.OpenInventoryMenu);
                }
                else if (_aMenuIsOpen)
                {
                    if (_inventoryMenuFocused)
                    {
                        _flowSubject.Notify(SubjectMessage.CloseInventoryMenu);
                    }
                }
                else if (_inEncounter)
                {
                    _encounterSubject.Notify(SubjectMessage.Cancel);
                }

                break;
            case SubjectMessage.StartEncounter:
                _inEncounter = true;
                break;
            case SubjectMessage.EndEncounter:
                _inEncounter = false;
                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case PlayerRequestsPrimaryActionEvent playerActionEvent when IsFreeRoaming():
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
                    var interactable = _positionGrid.Get(position[0], position[1]).GetComponent<Interactable>();

                    if (interactable != null)
                    {
                        var interactWithEvent = new InteractWith(interactable, playerActionEvent.Direction);
                        _flowSubject.Notify(interactWithEvent);
                    }
                }

                break;
            }
            case PlayerRequestsPrimaryActionEvent:
            {
                if (_messageBoxFocused)
                {
                    _flowSubject.Notify(SubjectMessage.AdvanceEvent);
                }
                else if(_inEncounter)
                {
                    _encounterSubject.Notify(SubjectMessage.MenuSelection);
                }
                else
                {
                    _flowSubject.Notify(SubjectMessage.MenuSelection);
                }

                break;
            }
        }
    }

    private bool IsFreeRoaming()
    {
        return !_inEvent && !_aMenuIsOpen && !_inEncounter;
    }
}