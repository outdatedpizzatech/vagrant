using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public InputAction InputAction = new InputAction();

    private void Update()
    {
        if (Input.GetAxis("Vertical") < 0)
        {
            InputAction.AddDirection(Enums.Direction.Down);
        }
        else
        {
            InputAction.InputDirections.Remove(Enums.Direction.Down);
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            InputAction.AddDirection(Enums.Direction.Up);
        }
        else
        {
            InputAction.InputDirections.Remove(Enums.Direction.Up);
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            InputAction.AddDirection(Enums.Direction.Left);
        }
        else
        {
            InputAction.InputDirections.Remove(Enums.Direction.Left);
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            InputAction.AddDirection(Enums.Direction.Right);
        }
        else
        {
            InputAction.InputDirections.Remove(Enums.Direction.Right);
        }

        InputAction.Acting = Input.GetKeyDown(KeyCode.Space);
    }
}
