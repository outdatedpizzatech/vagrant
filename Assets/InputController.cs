using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public readonly List<Enums.Direction> InputDirections = new List<Enums.Direction>();

    private void Update()
    {
        if (Input.GetAxis("Vertical") < 0)
        {
            AddDirection(Enums.Direction.Down);
        }
        else
        {
            InputDirections.Remove(Enums.Direction.Down);
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            AddDirection(Enums.Direction.Up);
        }
        else
        {
            InputDirections.Remove(Enums.Direction.Up);
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            AddDirection(Enums.Direction.Left);
        }
        else
        {
            InputDirections.Remove(Enums.Direction.Left);
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            AddDirection(Enums.Direction.Right);
        }
        else
        {
            InputDirections.Remove(Enums.Direction.Right);
        }
    }

    private void AddDirection(Enums.Direction direction)
    {
        if (!InputDirections.Contains(direction))
        {
            InputDirections.Add(direction);
        }
    }
}
