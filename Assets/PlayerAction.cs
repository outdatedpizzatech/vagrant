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

    private void Update()
    {
        if (_inputAction.Acting)
        {
            _actionSubject.Notify(new PlayerActionEvent(_personMovement.position[0], _personMovement.position[1],
                _personMovement.facingDirection));
        }
    }
}