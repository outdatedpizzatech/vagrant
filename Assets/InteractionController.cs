using System.Linq;
using UnityEngine;

public class InteractionController : MonoBehaviour, IObserver
{
    private enum State
    {
        Free,
        InDialogue
    }

    private Subject _interactionSubject;
    private Subject _flowSubject;
    private State _state = State.Free;
    private PositionGrid _positionGrid;
    private InputAction _inputAction;
    private float _timeSinceLastDirectionalInput;

    private void Update()
    {
        _timeSinceLastDirectionalInput += Time.deltaTime;
        
        void NotifyDialogueInputs()
        {
            if (_state == State.InDialogue && _inputAction.InputDirections.Any())
            {
                _flowSubject.Notify(new MenuNavigation(_inputAction.InputDirections.Last()));
            }
        }
        
        Utilities.Debounce(ref _timeSinceLastDirectionalInput, 0.05f, NotifyDialogueInputs);
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
            case SubjectMessage.EndDialogue:
                _state = State.Free;
                break;
        }
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case PlayerActionEvent playerActionEvent when _state == State.Free:
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
                        var receivedFromDirection = (Enums.Direction)(((int)playerActionEvent.Direction + 2) % 4);
                        _flowSubject.Notify(
                            new InteractionResponseEvent(interactable.ReceiveInteraction(receivedFromDirection)));
                    }
                }

                break;
            }
            case PlayerActionEvent:
            {
                if (_state == State.InDialogue)
                {
                    _flowSubject.Notify(SubjectMessage.AdvanceDialogue);
                }

                break;
            }
            case InteractionResponseEvent:
                _state = State.InDialogue;
                break;
        }
    }
}