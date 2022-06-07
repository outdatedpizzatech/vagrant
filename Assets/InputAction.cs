using System.Collections.Generic;

public class InputAction
{
    public readonly List<Enums.Direction> InputDirections = new List<Enums.Direction>();
    public bool Acting = false;
    
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