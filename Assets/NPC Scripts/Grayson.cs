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
            DontSeeCollection,
            TryingToGetOnGoodSide,
            NotTryingToGetOnGoodSide,
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

        public List<MessageEnvelope> ReceiveInteraction(object promptId)
        {
            switch (promptId)
            {
                case PromptKeys.SeeCollection:
                {
                    var response = new MessageEnvelope
                    {
                        Message = "Trying to get on my good side, eh?"
                    };
                    response.Prompts.Add(new Prompt(PromptKeys.TryingToGetOnGoodSide, "You got me"));
                    response.Prompts.Add(new Prompt(PromptKeys.NotTryingToGetOnGoodSide, "Of course not!"));
                    return new List<MessageEnvelope>
                    {
                        response
                    };
                }
                case PromptKeys.DontSeeCollection:
                {
                    var response = new MessageEnvelope
                    {
                        Message = "Oh, well... next time, perhaps."
                    };
                    return new List<MessageEnvelope>
                    {
                        response
                    };
                }
                case PromptKeys.TryingToGetOnGoodSide:
                {
                    var response = new MessageEnvelope
                    {
                        Message = "Hah! I may be old, but I'm still sharp!"
                    };
                    return new List<MessageEnvelope>
                    {
                        response
                    };
                }
                case PromptKeys.NotTryingToGetOnGoodSide:
                {
                    var response = new MessageEnvelope
                    {
                        Message = "Now, now... you're starting to embarrass yourself."
                    };
                    return new List<MessageEnvelope>
                    {
                        response
                    };
                }
            }

            return new List<MessageEnvelope>();
        }
    }
}