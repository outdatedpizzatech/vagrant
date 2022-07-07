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

            var response = new InteractionEvent();

            response.AddMessage("Are you prepared for the kind of death that you've earned?");

            response.Prompts.Add(new Prompt(PromptKeys.No, "No"));
            response.Prompts.Add(new Prompt(PromptKeys.Yes, "Yes"));
            
            return response;
        }

        public override InteractionEvent ReceiveInteraction(object promptId)
        {
            switch (promptId)
            {
                case PromptKeys.Yes:
                {
                    var response = new InteractionEvent();
                    response.AddMessage("Very well. Prepare yourselves!");
                    response.TriggersEncounter = true;
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