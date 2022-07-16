using UnityEngine;

public class Ability
{
    public enum TargetingMode
    {
        Single,
        Multiple,
        All
    }

    public string name;
    public string animationName;
    public TargetingMode targetingMode;

    public Ability(string _name, string _animationName, TargetingMode _targetingMode)
    {
        name = _name;
        animationName = _animationName;
        targetingMode = _targetingMode;
    }
}