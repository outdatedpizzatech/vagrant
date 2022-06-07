using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerWarp : MonoBehaviour, IObserver
{
    [CanBeNull] private Subject _subject;
    private PersonMovement _personMovement;
    private Animator _animator;

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
    
    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.PlayerRequestingWarp:
                _personMovement.CanMakeAnotherMove = false;
                break;
            case SubjectMessage.ScreenFinishedWipeOut:
                _personMovement.MoveTransformToPosition();
                _animator.SetInteger("facingDirection", (int)_personMovement.FacingDirection);
                _animator.SetBool("isMoving", false);
                break;
            case SubjectMessage.ScreenFinishedWipeIn:
                _personMovement.CanMakeAnotherMove = true;
                break;
        }
    }
    public void OnNotify<T>(T parameters)
    {
        if (parameters is PlayerBeganWarpingEvent playerBeganWarpingEvent)
        {
            _personMovement.FacingDirection = playerBeganWarpingEvent.FacingDirection;
            _personMovement.SetPosition(playerBeganWarpingEvent.X, playerBeganWarpingEvent.Y);
        }
    }
}
