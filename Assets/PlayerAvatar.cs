using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int FacingDirection = Animator.StringToHash("facingDirection");

    // Start is called before the first frame update
    void Start()
    {
        var animator = GetComponent<Animator>();
        animator.SetBool(IsMoving, true);
        animator.SetInteger(FacingDirection, (int)Enums.Direction.Up);
        animator.speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
    }
}