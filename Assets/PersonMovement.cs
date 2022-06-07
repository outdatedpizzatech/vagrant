using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PersonMovement : MonoBehaviour
{
    public float speedMultiplier = 4.5f;
    public bool CanMakeAnotherMove = true;
    private int[] Position = new int[2] { 0, 0 };
    public Enums.Direction? FacingDirection;
    
    private Collider2D myCollider;
    [CanBeNull] private List<Enums.Direction> _requestedDirections;
    private bool isMoving;
    private Animator _animator;
    private Subject _occupiedSpacesSubject;

    public void SetPosition(int x, int y)
    {
        _occupiedSpacesSubject.Notify(new LeftPositionEvent(this));
        Position[0] = x;
        Position[1] = y;
        _occupiedSpacesSubject.Notify(new EnteredPositionEvent(this));
    }

    public void MoveTransformToPosition()
    {
        transform.position = new Vector2(Position[0], Position[1]);
    }

    public void Setup(List<Enums.Direction> requestedDirections, Subject occupiedSpacesSubject)
    {
        _requestedDirections = requestedDirections;
        _occupiedSpacesSubject = occupiedSpacesSubject;
    }

    public void Start()
    {
        myCollider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        SetPosition((int)transform.position.x, (int)transform.position.y);
    }

    void Update()
    {
        if (_requestedDirections == null)
        {
            throw new Exception($"{name}: requestedDirections not supplied");
        }
        
        if (_occupiedSpacesSubject == null)
        {
            throw new Exception($"{name}: occupiedSpacesSubject not supplied");
        }

        if (!isMoving)
        {
            if (!CanMakeAnotherMove)
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
                
                FacingDirection = direction;
                
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

            if (FacingDirection != null)
            {
                _animator.SetInteger("facingDirection", (int)FacingDirection);
            }
            else
            {
                _animator.SetInteger("facingDirection", -1);
            }

            if (isMoving)
            {
                switch (FacingDirection)
                {
                    case Enums.Direction.Down:
                        SetPosition(Position[0], Position[1] - 1);
                        break;
                    case Enums.Direction.Up:
                        SetPosition(Position[0], Position[1] + 1);
                        break;
                    case Enums.Direction.Left:
                        SetPosition(Position[0] - 1, Position[1]);
                        break;
                    case Enums.Direction.Right:
                        SetPosition(Position[0] + 1, Position[1]);
                        break;
                }
            }

            return;
        }
        
        var destination = new Vector2(Position[0], Position[1]);
        var distance = destination - (Vector2)transform.position;

        if (
            (FacingDirection == Enums.Direction.Down && (distance.y > 0)) ||
            (FacingDirection == Enums.Direction.Up && distance.y < 0) ||
            (FacingDirection == Enums.Direction.Right && distance.x < 0) ||
            (FacingDirection == Enums.Direction.Left && distance.x > 0)
        )
        {
            transform.position = destination;
            isMoving = false;
        }
        else
        {
            transform.Translate(ToVector2((Enums.Direction)FacingDirection) * Time.deltaTime *
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
}
