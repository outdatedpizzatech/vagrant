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

            response.CanFollowUp = true;

            return response;
        }

        public InteractionEvent ReceiveInteraction(object promptId)
        {
            switch (promptId)
            {
                case Item item:
                {
                    if (item.itemName == "Hamburger")
                    {
                        var response = new InteractionEvent();
                        response.AddMessage("Oh, yummy! You shouldn't have!");
                        response.CanFollowUp = true;
                        return response;
                    }
                    else
                    {
                        var response = new InteractionEvent();
                        response.AddMessage("Thanks, but no thanks.");
                        response.CanFollowUp = true;
                        return response;
                    }
                }
            }

            return null;
        }
    }
}