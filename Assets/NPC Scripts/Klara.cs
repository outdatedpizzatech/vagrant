using System.Collections.Generic;
using UnityEngine;

namespace NPC_Scripts
{
    public class Klara : MonoBehaviour, IInteractable
    {
        private static readonly int FacingDirection = Animator.StringToHash("facingDirection");

        public InteractionEvent ReceiveInteraction(Enums.Direction direction)
        {
            var animator = GetComponent<Animator>();
            animator.SetInteger(FacingDirection, (int)direction);

            var response = new InteractionEvent();

            response.AddMessage("Be sure to visit the treasure vault on your way out.");

            return response;
        }

        public InteractionEvent ReceiveInteraction(object promptId)
        {
            return null;
        }
    }
}