using System;
using System.Collections.Generic;

internal static class Utilities
{
    public static void Debounce(ref float currentTime, float maxTime, Action doFunction)
    {
        if (!(currentTime > maxTime)) return;
        currentTime = 0;
        doFunction();
    }

    public static Enums.Direction GetOppositeDirection(Enums.Direction direction)
    {
        var newDirection = ((int)direction + 2) % 4;
        return (Enums.Direction)newDirection;
    }

    public static string PromptOutput(List<string> prompts, int selectedPromptIndex, bool isFocused)
    {
        var promptIndex = 0;
        var text = "";

        foreach (var prompt in prompts)
        {
            if (promptIndex == selectedPromptIndex)
            {
                if (isFocused)
                {
                    text += $"\n<sprite anim='0,1,4'> {prompt}";
                }else{
                    text += $"\n<sprite=0> {prompt}";
                }
            }
            else
            {
                text += $"\n<sprite=1> {prompt}";
            }

            promptIndex++;
        }

        return text;
    }
}