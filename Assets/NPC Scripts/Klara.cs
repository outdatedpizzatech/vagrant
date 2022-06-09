using System.Collections.Generic;
using UnityEngine;

namespace NPC_Scripts
{
    public class Klara : MonoBehaviour, IInteractable
    {
        private static readonly int FacingDirection = Animator.StringToHash("facingDirection");

        public List<MessageEnvelope> ReceiveInteraction(Enums.Direction direction)
        {
            var animator = GetComponent<Animator>();
            animator.SetInteger(FacingDirection, (int)direction);
        
            var response1 = new MessageEnvelope
            {
                Message = "Be sure to visit the treasure vault on your way out."
            };

            return new List<MessageEnvelope>
            {
                response1
            };
        }
    }
}
