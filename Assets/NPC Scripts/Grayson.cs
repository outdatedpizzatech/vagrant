using System.Collections.Generic;
using UnityEngine;

namespace NPC_Scripts
{
    public class Grayson : Interactable
    {
        public Sprite expression;
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

            var response = new InteractionEvent();
            response.AddMessage("I'm Grayson. As you can see, I'm pretty much just an old man.", expression);
            response.AddMessage("Would you like to see my collection of antique cards?", expression);

            response.Prompts.Add(new Prompt(PromptKeys.SeeCollection, "Yes"));
            response.Prompts.Add(new Prompt(PromptKeys.DontSeeCollection, "No"));

            return response;
        }

        public override InteractionEvent ReceiveInteraction(object promptId)
        {
            switch (promptId)
            {
                case PromptKeys.SeeCollection:
                {
                    var response = new InteractionEvent();
                    response.AddMessage("Trying to get on my good side, eh?", expression);
                    response.Prompts.Add(new Prompt(PromptKeys.TryingToGetOnGoodSide, "You got me"));
                    response.Prompts.Add(new Prompt(PromptKeys.NotTryingToGetOnGoodSide, "Of course not!"));
                    return response;
                }
                case PromptKeys.DontSeeCollection:
                {
                    var response = new InteractionEvent();
                    response.AddMessage("Oh, well... next time, perhaps.", expression);
                    response.CanFollowUp = true;
                    return response;
                }
                case PromptKeys.TryingToGetOnGoodSide:
                {
                    var response = new InteractionEvent();
                    response.AddMessage("Hah! I may be old, but I'm still sharp!", expression);
                    return response;
                }
                case PromptKeys.NotTryingToGetOnGoodSide:
                {
                    var response = new InteractionEvent();
                    response.AddMessage("Now, now... you're starting to embarrass yourself.", expression);
                    return response;
                }
                case Item item:
                {
                    if (item.itemName == "Old Socks")
                    {
                        var response = new InteractionEvent();
                        response.AddMessage("I thought I left those somewhere!", expression);
                        response.CanFollowUp = true;
                        return response;
                    }
                    else
                    {
                        var response = new InteractionEvent();
                        response.AddMessage("What is it? I can't see so well, y'know.", expression);
                        response.CanFollowUp = true;
                        return response;
                    }
                }
            }

            return null;
        }
    }
}