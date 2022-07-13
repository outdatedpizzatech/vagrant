using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int FacingDirection = Animator.StringToHash("facingDirection");
    public int hitPoints;

    private void Start()
    {
        var animator = GetComponent<Animator>();
        animator.SetBool(IsMoving, true);
        animator.SetInteger(FacingDirection, (int)Enums.Direction.Up);
        animator.speed = 0.5f;
    }
    
    public void ReceiveDamage(int damage)
    {
        hitPoints -= damage;
    }
}