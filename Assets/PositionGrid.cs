using System.Collections.Generic;
using UnityEngine;

public class PositionGrid
{
    private readonly Dictionary<string, GameObject> _grid = new();

    public void Add(int x, int y, GameObject value)
    {
        _grid.Add(Keyed(x, y), value);
    }
    
    public void Remove(int x, int y)
    {
        _grid.Remove(Keyed(x, y));
    }
    
    public bool Has(int x, int y)
    {
        return _grid.ContainsKey(Keyed(x, y));
    }

    public GameObject Get(int x, int y)
    {
        return _grid[Keyed(x, y)];
    }

    private static string Keyed(int x, int y)
    {
        return $"{x},{y}";
    }
}