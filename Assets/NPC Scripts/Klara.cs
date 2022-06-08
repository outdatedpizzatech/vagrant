using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Klara : MonoBehaviour, IInteractible
{
    public List<NPCResponse> ReceiveInteraction(Enums.Direction direction)
    {
        var animator = GetComponent<Animator>();
        animator.SetInteger("facingDirection", (int)direction);
        
        var response1 = new NPCResponse();
        response1.Message = "Be sure to visit the treasure vault on your way out.";
        
        return (new List<NPCResponse>
        {
            response1
        });
    }
}
