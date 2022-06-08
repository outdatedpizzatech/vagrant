using System.Collections.Generic;
using UnityEngine;

public class Grayson : MonoBehaviour, IInteractible
{
    public List<NPCResponse> ReceiveInteraction(Enums.Direction direction)
    {
        var animator = GetComponent<Animator>();
        animator.SetInteger("facingDirection", (int)direction);

        var response1 = new NPCResponse();
        response1.Message = "I'm Grayson. As you can see, I'm pretty much just an old man.";
        
        var response2 = new NPCResponse();
        response2.Message = "Would you like to see my collection of antique cards?";
        
        return (new List<NPCResponse>
        {
            response1, response2
        });
    }
}
