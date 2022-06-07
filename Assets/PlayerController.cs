using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void Setup(Subject warpAndWipeSubject, List<Enums.Direction> requestedDirections, Subject occupiedSpacesSubject)
    {
        var playerWarp = GetComponent<PlayerWarp>();
        var playerMovement = GetComponent<PersonMovement>();
        
        playerMovement.Setup(requestedDirections, occupiedSpacesSubject);
        playerWarp.Setup(warpAndWipeSubject);
    }
}
