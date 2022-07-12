using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private InputAction _inputAction;
    private PersonMovement _personMovement;
    private Subject _actionSubject;
    private float _timeSinceLastSecondaryInput;

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
        _timeSinceLastSecondaryInput += Time.deltaTime;

        if (_inputAction.Acting)
        {
            _actionSubject.Notify(new PlayerRequestsPrimaryActionEvent(_personMovement.position[0],
                _personMovement.position[1],
                _personMovement.facingDirection));
        }

        if (_inputAction.SecondaryActing)
        {
            void NotifySecondaryInput()
            {
                _actionSubject.Notify(GeneralTopic.PlayerRequestsSecondaryAction);
            }

            Utilities.Debounce(ref _timeSinceLastSecondaryInput, 0.2f, NotifySecondaryInput);
        }
    }
}