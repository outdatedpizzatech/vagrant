using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Klara : MonoBehaviour, IInteractible
{
    public string[] ReceiveInteraction(Enums.Direction direction)
    {
        var animator = GetComponent<Animator>();
        animator.SetInteger("facingDirection", (int)direction);
        
        return (new string[]
        {
            "Be sure to visit the treasure vault on your way out.",
        });
    }
}
