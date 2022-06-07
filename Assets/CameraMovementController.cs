using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    public PlayerMovement playerMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var newPosition = new Vector3(playerMovement.transform.position.x, playerMovement.transform.position.y, -10);
        transform.position = newPosition;
    }
}
