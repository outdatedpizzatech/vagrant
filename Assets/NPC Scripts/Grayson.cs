using System.Collections.Generic;
using UnityEngine;

public class Grayson : MonoBehaviour, IInteractible
{
    enum PromptKeys
    {
        SeeCollection,
        DontSeeCollection
    }
    
    public List<MessageEnvelope> ReceiveInteraction(Enums.Direction direction)
    {
        var animator = GetComponent<Animator>();
        animator.SetInteger("facingDirection", (int)direction);

        var response1 = new MessageEnvelope();
        response1.Message = "I'm Grayson. As you can see, I'm pretty much just an old man.";
        
        var response2 = new MessageEnvelope();
        response2.Message = "Would you like to see my collection of antique cards?";
        response2.Prompts.Add(new Prompt(PromptKeys.SeeCollection, "Yes"));
        response2.Prompts.Add(new Prompt(PromptKeys.DontSeeCollection, "No"));
        
        return (new List<MessageEnvelope>
        {
            response1, response2
        });
    }
}
