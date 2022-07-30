using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class EncounterLoader : MonoBehaviour
{
    public enum Encounterable
    {
        Cragman,
        Miasmadame
    }

    private static readonly Dictionary<Encounterable, Object> _registry = new ();

    public static Object Get(Encounterable encounterable)
    {
        return _registry[encounterable];
    }
    
    private void Awake()
    {
        var values = Enum.GetValues(typeof(Encounterable));

        foreach (Encounterable val in values)
        {
            _registry[val] = Resources.Load($"Encounters/{val.ToString()}");
        }
    }
}