using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private List<string> _items = new();
    
    public void Setup(Subject warpAndWipeSubject, InputAction inputAction, Subject occupiedSpacesSubject,
        PositionGrid positionGrid, Subject interactionSubject, Subject flowSubject)
    {
        var playerWarp = GetComponent<PlayerWarp>();
        var playerMovement = GetComponent<PersonMovement>();
        var playerAction = GetComponent<PlayerAction>();

        playerMovement.Setup(inputAction, occupiedSpacesSubject, positionGrid, flowSubject);
        playerWarp.Setup(warpAndWipeSubject);
        playerAction.Setup(inputAction, interactionSubject);
    }

    public void AddItem(string item)
    {
        _items.Add(item);
    }
}