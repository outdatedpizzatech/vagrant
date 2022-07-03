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

    private void Update()
    {
        _timeSinceLastDirectionalInput += Time.deltaTime;

        _inMenu = contextController.activeContexts.Any((x) =>
            x is Enums.ControlContext.InteractionMenu or Enums.ControlContext.InventoryMenu);
        _inEvent = contextController.activeContexts.Any((x) => x == Enums.ControlContext.Event);

        if (!_halted)
        {
            if (_inMenu || _inEvent)
            {
                _flowSubject.Notify(SubjectMessage.TimeShouldFreeze);
                _halted = true;
            }
        }
        else
        {
            if (!_inMenu && !_inEvent)
            {
                _flowSubject.Notify(SubjectMessage.TimeShouldFlow);
                _halted = false;
            }
        }

        if (contextController.activeContexts.Last() == Enums.ControlContext.InteractionMenu)
        {
            _flowSubject.Notify(SubjectMessage.GiveContextToInteractionMenu);
        }

        void NotifyMenuInputs()
        {
            if (!_inEvent && !_inMenu || !_inputAction.InputDirections.Any()) return;
            if (_currentControlContext() == Enums.ControlContext.InteractionMenu)
            {
                _flowSubject.Notify(new InteractionMenuNavigation(_inputAction.InputDirections.Last()));
            }
            else
            {
                _flowSubject.Notify(new InventoryMenuNavigation(_inputAction.InputDirections.Last()));
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
            case SubjectMessage.PlayerRequestsSecondaryActionEvent:
                if (!_inEvent && !_inMenu)
                {
                    _flowSubject.Notify(SubjectMessage.OpenInventoryMenu);
                }
                else if (_inMenu)
                {
                    if (contextController.activeContexts.Last() == Enums.ControlContext.InventoryMenu)
                    {
                        _flowSubject.Notify(SubjectMessage.CloseInventoryMenu);
                    }
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
                        var interactWithEvent = new InteractWith(interactable, playerActionEvent.Direction);
                        _flowSubject.Notify(interactWithEvent);
                    }
                }

                break;
            }
            case PlayerRequestsPrimaryActionEvent:
            {
                if (_currentControlContext() == Enums.ControlContext.Event)
                {
                    _flowSubject.Notify(SubjectMessage.AdvanceEvent);
                }
                else if (_currentControlContext() == Enums.ControlContext.InteractionMenu)
                {
                    _flowSubject.Notify(SubjectMessage.SelectInteractionMenuItem);
                }
                else if (_currentControlContext() == Enums.ControlContext.InventoryMenu)
                {
                    _flowSubject.Notify(SubjectMessage.SelectInventoryMenuItem);
                }

                break;
            }
        }
    }

    private Enums.ControlContext _currentControlContext()
    {
        return contextController.activeContexts.Count == 0
            ? Enums.ControlContext.None
            : contextController.activeContexts.Last();
    }
}