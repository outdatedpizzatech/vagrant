using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    public PersonMovement playerMovement;
    public float zPosition;

    private void LateUpdate()
    {
        var playerPosition = playerMovement.transform.position;
        var newPosition = new Vector3(playerPosition.x, playerPosition.y, zPosition);
        transform.position = newPosition;
    }
}
