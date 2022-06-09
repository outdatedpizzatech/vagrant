using System.Collections.Generic;
using UnityEngine;

namespace NPC_Scripts
{
    public class Grayson : MonoBehaviour, IInteractable
    {
        private static readonly int FacingDirection = Animator.StringToHash("facingDirection");

        private enum PromptKeys
        {
            SeeCollection,
            DontSeeCollection
        }
    
        public List<MessageEnvelope> ReceiveInteraction(Enums.Direction direction)
        {
            var animator = GetComponent<Animator>();
            animator.SetInteger(FacingDirection, (int)direction);

            var response1 = new MessageEnvelope
            {
                Message = "I'm Grayson. As you can see, I'm pretty much just an old man."
            };

            var response2 = new MessageEnvelope
            {
                Message = "Would you like to see my collection of antique cards?"
            };
            response2.Prompts.Add(new Prompt(PromptKeys.SeeCollection, "Yes"));
            response2.Prompts.Add(new Prompt(PromptKeys.DontSeeCollection, "No"));
        
            return new List<MessageEnvelope>
            {
                response1, response2
            };
        }
    }
}
