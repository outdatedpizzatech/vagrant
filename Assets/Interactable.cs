using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private readonly List<Item> _items = new();

    public InteractionEvent ReceiveItem(Item item)
    {
        _items.Add(item);
        return (ReceiveInteraction(item));
    }

    public virtual InteractionEvent ReceiveInteraction(Enums.Direction direction)
    {
        return null;
    }

    public virtual InteractionEvent ReceiveInteraction(object promptId)
    {
        return null;
    }
}