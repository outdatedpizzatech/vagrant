using UnityEngine;

public class PartyAvatar : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int FacingDirection = Animator.StringToHash("facingDirection");
    private Damageable _damageable;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool(IsMoving, true);
        _animator.SetInteger(FacingDirection, (int)Enums.Direction.Up);
        _animator.speed = 0.5f;
        _damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        _animator.SetBool(IsMoving, _damageable.hitPoints > 0);
    }
}