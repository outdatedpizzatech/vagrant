using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC_Scripts
{
    public class Klara : Interactable
    {
        public AnimationClip neutralSpeakingAnimation;
        public AnimationClip neutralIdleAnimation;
        private static readonly int FacingDirection = Animator.StringToHash("facingDirection");

        public override InteractionEvent ReceiveInteraction(Enums.Direction direction)
        {
            var animator = GetComponent<Animator>();
            animator.SetInteger(FacingDirection, (int)direction);

            var response = new InteractionEvent(PostEvent.CanFollowUp);

            response.AddMessage("Be sure to visit the treasure vault on your way out.", neutralSpeakingAnimation, neutralIdleAnimation);
            response.AddMessage("Doot doot doot doot doot doot doot doot doot doot.", neutralSpeakingAnimation, neutralIdleAnimation);
            
            return response;
        }

        public override InteractionEvent ReceiveInteraction(object promptId)
        {
            switch (promptId)
            {
                case Item item:
                {
                    if (item.itemName == "Hamburger")
                    {
                        var response = new InteractionEvent(PostEvent.CanFollowUp);
                        response.AddMessage("Oh, yummy! You shouldn't have!", neutralSpeakingAnimation, neutralIdleAnimation);
                        return response;
                    }
                    else
                    {
                        var response = new InteractionEvent(PostEvent.CanFollowUp);
                        response.AddMessage("Thanks, but no thanks.", neutralSpeakingAnimation, neutralIdleAnimation);
                        return response;
                    }
                }
            }

            return null;
        }
    }
}