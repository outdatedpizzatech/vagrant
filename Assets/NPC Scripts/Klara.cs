using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Klara : MonoBehaviour, IInteractible
{
    public List<MessageEnvelope> ReceiveInteraction(Enums.Direction direction)
    {
        var animator = GetComponent<Animator>();
        animator.SetInteger("facingDirection", (int)direction);
        
        var response1 = new MessageEnvelope();
        response1.Message = "Be sure to visit the treasure vault on your way out.";
        
        return (new List<MessageEnvelope>
        {
            response1
        });
    }
}
