using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC_Scripts
{
    public class Keever : Interactable
    {
        public AnimationClip neutralSpeakingAnimation;
        public AnimationClip neutralIdleAnimation;
        private static readonly int FacingDirection = Animator.StringToHash("facingDirection");
        
        private enum PromptKeys
        {
            Yes,
            No,
        }

        public override InteractionEvent ReceiveInteraction(Enums.Direction direction)
        {
            var animator = GetComponent<Animator>();
            animator.SetInteger(FacingDirection, (int)direction);

            var prompts = new List<Prompt>
            {
                new(PromptKeys.No, "No"),
                new(PromptKeys.Yes, "Yes")
            };

            var response = new InteractionEvent(prompts);

            response.AddMessage("Are you prepared for the kind of death that you've earned?");
            
            return response;
        }

        public override InteractionEvent ReceiveInteraction(object promptId)
        {
            switch (promptId)
            {
                case PromptKeys.Yes:
                {
                    var response = new InteractionEvent(PostEvent.TriggersEncounter);
                    response.AddMessage("Very well. Prepare yourselves!");
                    return response;
                }
                case PromptKeys.No:
                {
                    var response = new InteractionEvent();
                    response.AddMessage("There's no shame in embracing your fear.");
                    return response;
                }
            }

            return null;
        }
    }
}