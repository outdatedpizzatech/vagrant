using System;

class Utilities
{
    public static void Debounce(ref float currentTime, float maxTime, Action doFunction)
    {
        if (currentTime > maxTime)
        {
            currentTime = 0;
            doFunction();
        }
    }
}