using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private InputAction _inputAction;
    private PersonMovement _personMovement;
    private Subject _actionSubject;

    private void Awake()
    {
        _personMovement = GetComponent<PersonMovement>();
    }
    
    public void Setup(InputAction inputAction, Subject interactionActionSubject)
    {
        _inputAction = inputAction;
        _actionSubject = interactionActionSubject;
    }

    void Update()
    {
        if (_inputAction.Acting)
        {
            _actionSubject.Notify(new PlayerActionEvent(_personMovement.Position[0], _personMovement.Position[1], _personMovement.FacingDirection));
        }
    }
}
