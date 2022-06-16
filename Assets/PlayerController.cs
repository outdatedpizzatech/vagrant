using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IObserver
{
    public List<string> _items = new();
    
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

    public void AddItem(string item)
    {
        _items.Add(item);
    }

    public List<string> Items()
    {
        return _items;
    }
    
    public void OnNotify<T>(T parameters)
    {
        switch (parameters)
        {
            case ReceiveItemEvent receiveItemEvent:
                AddItem(receiveItemEvent.Item);

                break;
        }
    }

    public void OnNotify(SubjectMessage message)
    {
    }
}