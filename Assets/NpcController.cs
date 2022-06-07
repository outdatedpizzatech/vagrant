using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public readonly List<Enums.Direction> InputDirections = new List<Enums.Direction>();
    private float timeTilNextMove;

    public void Setup(Subject occupiedSpaces, PositionGrid positionGrid)
    {
        var npcMovement = GetComponent<PersonMovement>();
        npcMovement.Setup(InputDirections, occupiedSpaces, positionGrid);
    }
    
    void Update()
    {
        timeTilNextMove -= Time.deltaTime;

        if (timeTilNextMove < 0)
        {
            timeTilNextMove = Random.Range(0f, 5f);

            InputDirections.Clear();
            InputDirections.Add((Enums.Direction)Random.Range(0, 3));
        }
    }
}
