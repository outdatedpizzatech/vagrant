using System.Collections.Generic;

public class InputAction
{
    public readonly List<Enums.Direction> InputDirections = new();
    public bool Acting = false;
    public bool SecondaryActing = false;
    
    public void AddDirection(Enums.Direction direction)
    {
        if (!InputDirections.Contains(direction))
        {
            InputDirections.Add(direction);
        }
    }
    
    public void ClearDirections()
    {
        InputDirections.Clear();
    }
}