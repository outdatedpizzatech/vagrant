using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PersonMovement : MonoBehaviour, IObserver
{
    public float speedMultiplier = 4.5f;
    public bool canMakeAnotherMove = true;
    public int[] position = { 0, 0 };
    public Enums.Direction facingDirection = Enums.Direction.Down;
    public bool marchingInPlace;
    public bool corporeal;

    private Collider2D _myCollider;
    private InputAction _inputAction;
    private bool _isMoving;
    private Animator _animator;
    private Subject _occupiedSpacesSubject;
    private Subject _flowSubject;
    private PositionGrid _positionGrid;
    private bool _isPlayer;

    private float _tempAnimationSpeed;
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int FacingDirection = Animator.StringToHash("facingDirection");

    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case FlowTopic.TimeShouldFlow:
                canMakeAnotherMove = true;
                _animator.speed = _tempAnimationSpeed;
                break;
            case FlowTopic.TimeShouldFreeze:
                canMakeAnotherMove = false;
                _tempAnimationSpeed = _animator.speed;
                _animator.speed = 0;
                break;
        }
    }

    public void RefreshPosition(int x, int y)
    {
        if (corporeal)
        {
            _occupiedSpacesSubject.Notify(new LeftPositionEvent(position[0], position[1], _isPlayer));
        }

        SetPosition(x, y);
    }

    public void MoveTransformToPosition()
    {
        transform.position = new Vector2(position[0], position[1]);
    }

    public void Setup(InputAction inputAction, Subject occupiedSpacesSubject, PositionGrid positionGrid,
        Subject flowSubject)
    {
        _inputAction = inputAction;
        _occupiedSpacesSubject = occupiedSpacesSubject;
        _flowSubject = flowSubject;
        _positionGrid = positionGrid;

        _flowSubject.AddObserver(this);
    }

    public void Start()
    {
        _myCollider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        var currentPosition = transform.position;
        SetPosition((int)currentPosition.x, (int)currentPosition.y);
    }

    private void Awake()
    {
        _isPlayer = GetComponent<PlayerController>() != null;
    }

    private void Update()
    {
        if (!_isMoving)
        {
            if (!canMakeAnotherMove)
            {
                return;
            }

            for (var i = _inputAction.InputDirections.Count - 1; i >= 0; i--)
            {
                if (_isMoving)
                {
                    break;
                }

                var direction = _inputAction.InputDirections[i];

                facingDirection = direction;

                if (WouldBeObstructed(direction)) continue;
                _isMoving = true;
                _animator.SetBool(IsMoving, true);
            }

            _animator.SetInteger(FacingDirection, (int)facingDirection);

            switch (_isMoving)
            {
                case false:
                    _animator.SetBool(IsMoving, marchingInPlace);
                    if (marchingInPlace)
                    {
                        _animator.speed = 0.5f;
                    }

                    break;
                case true:
                    _animator.speed = 1;

                    switch (facingDirection)
                    {
                        case Enums.Direction.Down:
                            RefreshPosition(position[0], position[1] - 1);
                            break;
                        case Enums.Direction.Up:
                            RefreshPosition(position[0], position[1] + 1);
                            break;
                        case Enums.Direction.Left:
                            RefreshPosition(position[0] - 1, position[1]);
                            break;
                        case Enums.Direction.Right:
                            RefreshPosition(position[0] + 1, position[1]);
                            break;
                    }

                    break;
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
            _isMoving = false;
        }
        else
        {
            transform.Translate(ToVector2(facingDirection) * (Time.deltaTime * speedMultiplier));
        }
    }

    private static Vector2 ToVector2(Enums.Direction direction)
    {
        return direction switch
        {
            Enums.Direction.Up => Vector2.up,
            Enums.Direction.Down => Vector2.down,
            Enums.Direction.Left => Vector2.left,
            _ => Vector2.right
        };
    }

    private bool WouldBeObstructed(Enums.Direction direction)
    {
        if (!corporeal)
        {
            return false;
        }
        
        _myCollider.enabled = false;

        var nextPosition = new int[2];
        position.CopyTo(nextPosition, 0);

        switch (facingDirection)
        {
            case Enums.Direction.Down:
                nextPosition[1] -= 1;
                break;
            case Enums.Direction.Up:
                nextPosition[1] += 1;
                break;
            case Enums.Direction.Left:
                nextPosition[0] -= 1;
                break;
            case Enums.Direction.Right:
                nextPosition[0] += 1;
                break;
        }

        if (_positionGrid.Has(nextPosition[0], nextPosition[1]))
        {
            return true;
        }

        var obstruction = Physics2D.Raycast(
            (Vector2)transform.position + _myCollider.offset,
            ToVector2(direction), 1).collider;
        var isObstructive = obstruction != null && obstruction.GetComponent<Doorway>() == null;
        _myCollider.enabled = true;
        return isObstructive;
    }

    private void SetPosition(int x, int y)
    {
        if (corporeal)
        {
            _occupiedSpacesSubject.Notify(new EnteredPositionEvent(this.gameObject, x, y));
        }

        position[0] = x;
        position[1] = y;
    }
}