using System;
using System.Collections;
using UnityEngine;

public class PartyMemberController : MonoBehaviour, IObserver
{
    private readonly InputAction _inputAction = new();
    private PersonMovement _personMovement;

    private void Awake()
    {
        _personMovement = GetComponent<PersonMovement>();
    }

    public void Setup(Subject occupiedSpaces, PositionGrid positionGrid, Subject flowSubject)
    {
        var npcMovement = GetComponent<PersonMovement>();
        npcMovement.Setup(_inputAction, occupiedSpaces, positionGrid, flowSubject);

        occupiedSpaces.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case LeftPositionEvent leftPositionEvent:
                if (leftPositionEvent.IsPlayer)
                {
                    if (Math.Abs(leftPositionEvent.X - _personMovement.position[0]) +
                        Math.Abs(leftPositionEvent.Y - _personMovement.position[1]) > 1)
                    {
                        _personMovement.RefreshPosition(leftPositionEvent.X, leftPositionEvent.Y);
                        _personMovement.MoveTransformToPosition();
                    }
                    else
                    {
                        if (leftPositionEvent.X > _personMovement.position[0])
                        {
                            _inputAction.AddDirection(Enums.Direction.Right);
                        }
                        else if (leftPositionEvent.X < _personMovement.position[0])
                        {
                            _inputAction.AddDirection(Enums.Direction.Left);
                        }
                        else if (leftPositionEvent.Y > _personMovement.position[1])
                        {
                            _inputAction.AddDirection(Enums.Direction.Up);
                        }
                        else if (leftPositionEvent.Y < _personMovement.position[1])
                        {
                            _inputAction.AddDirection(Enums.Direction.Down);
                        }
                    }
                }

                break;
        }
    }

    private void Update()
    {
        if (_inputAction.InputDirections.Count > 0)
        {
            // simulate easing up on input after several frames
            StartCoroutine(ClearDirections(0.1f));
        }
    }

    private IEnumerator ClearDirections(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _inputAction.ClearDirections();
    }
}