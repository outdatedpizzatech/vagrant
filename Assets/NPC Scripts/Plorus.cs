using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC_Scripts
{
    public class Plorus : Interactable
    {
        public AnimationClip neutralSpeakingAnimation;
        public AnimationClip neutralIdleAnimation;
        private static readonly int FacingDirection = Animator.StringToHash("facingDirection");

        public override InteractionEvent ReceiveInteraction(Enums.Direction direction)
        {
            var encounter = new Encounter();
            encounter.Add(EncounterLoader.Encounterable.Cragman);
            encounter.Add(EncounterLoader.Encounterable.Cragman);
            var response = new InteractionEvent(encounter);
            response.AddMessage("I'm not interested in speaking. Have at you!");
            return response;
        }
    }
}