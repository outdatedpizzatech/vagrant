using System.Collections.Generic;
using UnityEngine;

namespace NPC_Scripts
{
    public class Grayson : Interactable
    {
        public AnimationClip neutralIdleAnimation;
        private static readonly int FacingDirection = Animator.StringToHash("facingDirection");

        private enum PromptKeys
        {
            SeeCollection,
            DontSeeCollection,
            TryingToGetOnGoodSide,
            NotTryingToGetOnGoodSide,
        }

        public override InteractionEvent ReceiveInteraction(Enums.Direction direction)
        {
            var animator = GetComponent<Animator>();
            animator.SetInteger(FacingDirection, (int)direction);

            var prompts = new List<Prompt>
            {
                new(PromptKeys.SeeCollection, "Yes"),
                new(PromptKeys.DontSeeCollection, "No")
            };

            var response = new InteractionEvent(prompts);
            
            response.AddMessage("I'm Grayson. As you can see, I'm pretty much just an old man.", neutralIdleAnimation, neutralIdleAnimation);
            response.AddMessage("Would you like to see my collection of antique cards?", neutralIdleAnimation, neutralIdleAnimation);


            return response;
        }

        public override InteractionEvent ReceiveInteraction(object promptId)
        {
            switch (promptId)
            {
                case PromptKeys.SeeCollection:
                {
                    var prompts = new List<Prompt>
                    {
                        new(PromptKeys.TryingToGetOnGoodSide, "You got me"),
                        new(PromptKeys.NotTryingToGetOnGoodSide, "Of course not!")
                    };
                    var response = new InteractionEvent(prompts);
                    response.AddMessage("Trying to get on my good side, eh?", neutralIdleAnimation, neutralIdleAnimation);
                    return response;
                }
                case PromptKeys.DontSeeCollection:
                {
                    var response = new InteractionEvent(PostEvent.CanFollowUp);
                    response.AddMessage("Oh, well... next time, perhaps.", neutralIdleAnimation, neutralIdleAnimation);
                    return response;
                }
                case PromptKeys.TryingToGetOnGoodSide:
                {
                    var response = new InteractionEvent();
                    response.AddMessage("Hah! I may be old, but I'm still sharp!", neutralIdleAnimation, neutralIdleAnimation);
                    return response;
                }
                case PromptKeys.NotTryingToGetOnGoodSide:
                {
                    var response = new InteractionEvent();
                    response.AddMessage("Now, now... you're starting to embarrass yourself.", neutralIdleAnimation, neutralIdleAnimation);
                    return response;
                }
                case Item item:
                {
                    if (item.itemName == "Old Socks")
                    {
                        var response = new InteractionEvent(PostEvent.CanFollowUp);
                        response.AddMessage("I thought I left those somewhere!", neutralIdleAnimation, neutralIdleAnimation);
                        return response;
                    }
                    else
                    {
                        var response = new InteractionEvent(PostEvent.CanFollowUp);
                        response.AddMessage("What is it? I can't see so well, y'know.", neutralIdleAnimation, neutralIdleAnimation);
                        return response;
                    }
                }
            }

            return null;
        }
    }
}