using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void Setup(Subject warpAndWipeSubject, InputAction inputAction, Subject occupiedSpacesSubject, PositionGrid positionGrid)
    {
        var playerWarp = GetComponent<PlayerWarp>();
        var playerMovement = GetComponent<PersonMovement>();
        var playerAction = GetComponent<PlayerAction>();
        
        playerMovement.Setup(inputAction, occupiedSpacesSubject, positionGrid);
        playerWarp.Setup(warpAndWipeSubject);
        playerAction.Setup(inputAction);
    }
}
