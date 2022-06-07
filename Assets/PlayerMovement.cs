using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour, IObserver
{
    public float speedMultiplier = 4.5f;
    
    private int[] position = new int[2] { 0, 0 };
    private Collider2D myCollider;
    [CanBeNull] private List<Enums.Direction> _requestedDirections;
    [CanBeNull] private Subject _subject;
    private Enums.Direction? facingDirection;
    private bool isMoving;
    private bool canMakeAnotherMove = true;
    private Animator _animator;

    public void Setup(List<Enums.Direction> requestedDirections, Subject subject)
    {
        _requestedDirections = requestedDirections;
        _subject = subject;
    }

    public void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_requestedDirections == null)
        {
            throw new Exception("requestedDirections not supplied");
        }
        
        if (_subject == null)
        {
            throw new Exception("subject not supplied");
        }

        if (!isMoving)
        {
            if (!canMakeAnotherMove)
            {
                return;
            }

            for (var i = _requestedDirections.Count - 1; i >= 0; i--)
            {
                if (isMoving)
                {
                    break;
                }

                var direction = _requestedDirections[i];
                
                facingDirection = direction;
                
                if (!WouldBeObstructed(direction))
                {
                    isMoving = true;
                    _animator.SetBool("isMoving", true);
                }
            }

            if (!isMoving)
            {
                _animator.SetBool("isMoving", false);
            }

            if (facingDirection != null)
            {
                _animator.SetInteger("facingDirection", (int)facingDirection);
            }
            else
            {
                _animator.SetInteger("facingDirection", -1);
            }

            if (isMoving)
            {
                switch (facingDirection)
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
            }

            return;
        }
        
        var destination = new Vector2(position[0], position[1]);
        var distance = destination - (Vector2)transform.position;

        if (
            (facingDirection == Enums.Direction.Down && (distance.y > 0)) ||
            (facingDirection == Enums.Direction.Up && distance.y < 0) ||
            (facingDirection == Enums.Direction.Right && distance.x < 0) ||
            (facingDirection == Enums.Direction.Left && distance.x > 0)
        )
        {
            transform.position = destination;
            isMoving = false;
        }
        else
        {
            transform.Translate(ToVector2((Enums.Direction)facingDirection) * Time.deltaTime *
                                speedMultiplier);
        }
    }

    private Vector2 ToVector2(Enums.Direction direction)
    {
        if (direction == Enums.Direction.Up)
        {
            return Vector2.up;
        }

        if (direction == Enums.Direction.Down)
        {
            return Vector2.down;
        }

        if (direction == Enums.Direction.Left)
        {
            return Vector2.left;
        }

        return Vector2.right;
    }

    private bool WouldBeObstructed(Enums.Direction direction)
    {
        myCollider.enabled = false;
        var obstruction = Physics2D.Raycast(
            (Vector2)transform.position + myCollider.offset,
            ToVector2(direction), 1).collider;
        var isObstructive = obstruction != null && obstruction.GetComponent<Doorway>() == null;
        myCollider.enabled = true;
        return isObstructive;
    }
    
    public void OnNotify(SubjectMessage message)
    {
        switch (message)
        {
            case SubjectMessage.PlayerRequestingWarp:
                canMakeAnotherMove = false;
                break;
            case SubjectMessage.ScreenFinishedWipeOut:
                transform.position = new Vector2(position[0], position[1]);
                _animator.SetInteger("facingDirection", (int)facingDirection);
                _animator.SetBool("isMoving", false);
                break;
            case SubjectMessage.ScreenFinishedWipeIn:
                canMakeAnotherMove = true;
                break;
        }
    }
    public void OnNotify<T>(T parameters)
    {
        if (parameters is PlayerBeganWarpingEvent playerBeganWarpingEvent)
        {
            facingDirection = playerBeganWarpingEvent.FacingDirection;
            position[0] = playerBeganWarpingEvent.X;
            position[1] = playerBeganWarpingEvent.Y;
        }
    }
}