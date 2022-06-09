using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    private float timeSinceLastDirectionalInput;
    private float timeBetweenDirectionalInputs = 0.05f;

    private void Update()
    {
        if (timeSinceLastDirectionalInput > timeBetweenDirectionalInputs)
        {
            timeSinceLastDirectionalInput = 0;
            if (_state == State.InDialogue && _inputAction.InputDirections.Any())
            {
                _flowSubject.Notify(new MenuNavigation(_inputAction.InputDirections.Last()));
            }
        }

        timeSinceLastDirectionalInput += Time.deltaTime;
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
        if (parameters is PlayerActionEvent playerActionEvent)
        {
            if (_state == State.Free)
            {
                int[] position = new int[] { playerActionEvent.X, playerActionEvent.Y };

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
                    var interactible = _positionGrid.Get(position[0], position[1]).GetComponent<IInteractible>();

                    if (interactible != null)
                    {
                        var receivedFromDirection = (Enums.Direction)(((int)playerActionEvent.Direction + 2) % 4);
                        _flowSubject.Notify(
                            new InteractionResponseEvent(interactible.ReceiveInteraction(receivedFromDirection)));
                    }
                }
            }
            else if (_state == State.InDialogue)
            {
                _flowSubject.Notify(SubjectMessage.AdvanceDialogue);
            }
        }

        if (parameters is InteractionResponseEvent)
        {
            _state = State.InDialogue;
        }
    }
}