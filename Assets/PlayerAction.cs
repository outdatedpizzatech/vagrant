using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private InputAction _inputAction;

    public void Setup(InputAction inputAction)
    {
        _inputAction = inputAction;
    }

    // Update is called once per frame
    void Update()
    {
        print("InputAction " + _inputAction.Acting);
    }
}
