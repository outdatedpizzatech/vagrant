using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    private InputAction _inputAction = new InputAction();
    private float timeTilNextMove;

    public void Setup(Subject occupiedSpaces, PositionGrid positionGrid)
    {
        var npcMovement = GetComponent<PersonMovement>();
        npcMovement.Setup(_inputAction, occupiedSpaces, positionGrid);
    }
    
    void Update()
    {
        timeTilNextMove -= Time.deltaTime;

        if (timeTilNextMove < 0)
        {
            timeTilNextMove = Random.Range(0f, 5f);

            _inputAction.ClearDirections();
            _inputAction.AddDirection((Enums.Direction)Random.Range(0, 3));
        }
    }
}
