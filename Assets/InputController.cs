using UnityEngine;

public class InputController : MonoBehaviour
{
    public readonly InputAction InputAction = new();

    private void Update()
    {
        if (Input.GetAxis("Vertical") < 0)
        {
            InputAction.AddDirection(Enums.Direction.Down);
            InputAction.InputDirections.Remove(Enums.Direction.Up);
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            InputAction.AddDirection(Enums.Direction.Up);
            InputAction.InputDirections.Remove(Enums.Direction.Down);
        }
        else
        {
            InputAction.InputDirections.Remove(Enums.Direction.Down);
            InputAction.InputDirections.Remove(Enums.Direction.Up);
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            InputAction.AddDirection(Enums.Direction.Left);
            InputAction.InputDirections.Remove(Enums.Direction.Right);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            InputAction.AddDirection(Enums.Direction.Right);
            InputAction.InputDirections.Remove(Enums.Direction.Left);
        }
        else
        {
            InputAction.InputDirections.Remove(Enums.Direction.Left);
            InputAction.InputDirections.Remove(Enums.Direction.Right);
        }

        InputAction.Acting = Input.GetKeyDown(KeyCode.Space);
        InputAction.SecondaryActing = Input.GetKeyDown(KeyCode.M);
    }
}