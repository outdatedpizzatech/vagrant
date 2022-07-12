using System.Collections;
using UnityEngine;

public class PlayerWarp : MonoBehaviour, IObserver
{
    private Subject _subject;
    private PersonMovement _personMovement;
    private Animator _animator;
    private static readonly int FacingDirection = Animator.StringToHash("facingDirection");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private void Awake()
    {
        _personMovement = GetComponent<PersonMovement>();
        _animator = GetComponent<Animator>();
    }

    public void Setup(Subject warpAndWipeSubject)
    {
        _subject = warpAndWipeSubject;
        warpAndWipeSubject.AddObserver(this);
    }

    public void WarpTo(int x, int y, Enums.Direction direction)
    {
        _subject.Notify(SubjectMessage.PlayerRequestingWarp);
        StartCoroutine(BeginWarping(0.25f, x, y, direction));
    }

    private IEnumerator BeginWarping(float waitTime, int x, int y, Enums.Direction direction)
    {
        yield return new WaitForSeconds(waitTime);

        /*
         * TODO: this is pretty sketchy. We're deferring the position setting because it
         * causes a bug if it's done immediately. This is a race condition waiting to happen.
         */

        var parameters = new PlayerBeganWarpingEvent(direction, x, y);

        _subject.Notify(parameters);
    }

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case PlayerBeganWarpingEvent playerBeganWarpingEvent:
                _personMovement.facingDirection = playerBeganWarpingEvent.FacingDirection;
                _personMovement.SetPosition(playerBeganWarpingEvent.X, playerBeganWarpingEvent.Y);
                break;
            case SubjectMessage.PlayerRequestingWarp:
                _personMovement.canMakeAnotherMove = false;
                break;
            case SubjectMessage.ScreenFinishedWipeOut:
                _personMovement.MoveTransformToPosition();
                _animator.SetInteger(FacingDirection, (int)_personMovement.facingDirection);
                _animator.SetBool(IsMoving, false);
                break;
            case SubjectMessage.ScreenFinishedWipeIn:
                _personMovement.canMakeAnotherMove = true;
                break;
        }
    }
}