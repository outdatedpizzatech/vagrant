using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour, IObserver
{
    public WindowController windowController;
    public float menuDirectionalDebounceTiming;

    private bool _inEncounter;
    private bool _aMenuIsOpen;
    private bool _halted;
    private Subject _interactionSubject;
    private Subject _flowSubject;
    private PositionGrid _positionGrid;
    private InputAction _inputAction;
    private float _timeSinceLastDirectionalInput;
    private bool _inventoryMenuFocused;
    private bool _messageBoxFocused;
    private bool _interactionMenuFocused;
    private bool _requestedInteractionMenuFocus;
    private bool _requestedInventoryMenuFocus;

    private void Update()
    {
        _timeSinceLastDirectionalInput += Time.deltaTime;

        if (!_halted)
        {
            if (!IsFreeRoaming())
            {
                _flowSubject.Notify(FlowTopic.TimeShouldFreeze);
                _halted = true;
            }
        }
        else
        {
            if (IsFreeRoaming())
            {
                _flowSubject.Notify(FlowTopic.TimeShouldFlow);
                _halted = false;
            }
        }

        void NotifyMenuInputs()
        {
            if (IsFreeRoaming() || !_inputAction.InputDirections.Any()) return;
            
            _interactionSubject.Notify(new MenuNavigation(_inputAction.InputDirections.Last()));
        }

        Utilities.Debounce(ref _timeSinceLastDirectionalInput, menuDirectionalDebounceTiming, NotifyMenuInputs);
    }

    public void Setup(Subject interactionSubject, PositionGrid positionGrid, Subject flowSubject,
        InputAction inputAction, Subject encounterSubject, Subject windowSubject)
    {
        _interactionSubject = interactionSubject;
        _positionGrid = positionGrid;
        _flowSubject = flowSubject;
        _inputAction = inputAction;

        _interactionSubject.AddObserver(this);
        _flowSubject.AddObserver(this);

        windowController.Setup(windowSubject);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case FlowTopic.StartEncounter:
                _inEncounter = true;
                break;
            case FlowTopic.EndEncounter:
                _inEncounter = false;
                break;
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
        }
    }

    private bool IsFreeRoaming()
    {
        return !windowController.Any() && !_inEncounter;
    }
}