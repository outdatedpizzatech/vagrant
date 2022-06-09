using System;

internal static class Utilities
{
    public static void Debounce(ref float currentTime, float maxTime, Action doFunction)
    {
        if (!(currentTime > maxTime)) return;
        currentTime = 0;
        doFunction();
    }
}