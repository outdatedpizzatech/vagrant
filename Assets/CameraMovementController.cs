using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    public PersonMovement playerMovement;

    private void LateUpdate()
    {
        var playerPosition = playerMovement.transform.position;
        var newPosition = new Vector3(playerPosition.x, playerPosition.y, -10);
        transform.position = newPosition;
    }
}
