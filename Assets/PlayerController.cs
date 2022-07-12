using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IObserver
{
    private readonly List<Item> _items = new();
    
    public void Setup(Subject warpAndWipeSubject, InputAction inputAction, Subject occupiedSpacesSubject,
        PositionGrid positionGrid, Subject interactionSubject, Subject flowSubject)
    {
        var playerWarp = GetComponent<PlayerWarp>();
        var playerMovement = GetComponent<PersonMovement>();
        var playerAction = GetComponent<PlayerAction>();

        playerMovement.Setup(inputAction, occupiedSpacesSubject, positionGrid, flowSubject);
        playerWarp.Setup(warpAndWipeSubject);
        playerAction.Setup(inputAction, interactionSubject);
        
        flowSubject.AddObserver(this);
    }

    public List<Item> Items()
    {
        return _items;
    }
    
    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case ReceiveItem receiveItemEvent:
                AddItem(receiveItemEvent.Item);

                break;
            case LoseItem loseItemEvent:
                RemoveItem(loseItemEvent.Item);

                break;
        }
    }

    public void OnNotify(SubjectMessage message)
    {
    }

    private void AddItem(Item item)
    {
        _items.Add(item);
    }

    private void RemoveItem(Item item)
    {
        _items.Remove(item);
    }
}