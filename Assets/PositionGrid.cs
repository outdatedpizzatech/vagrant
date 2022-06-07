using System.Collections.Generic;
using UnityEngine;

public class PositionGrid
{
    private Dictionary<string, GameObject> _grid = new Dictionary<string, GameObject>();

    public void Add(int x, int y, GameObject value)
    {
        _grid.Add($"{x},{y}", value);
    }
    
    public void Remove(int x, int y)
    {
        _grid.Remove($"{x},{y}");
    }
    
    public bool Has(int x, int y)
    {
        return _grid.ContainsKey($"{x},{y}");
    }

}