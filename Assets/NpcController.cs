using UnityEngine;

public class NpcController : MonoBehaviour
{
    private readonly InputAction _inputAction = new();
    private float _timeTilNextMove;
    public bool autoMove;

    public void Setup(Subject occupiedSpaces, PositionGrid positionGrid, Subject flowSubject)
    {
        var npcMovement = GetComponent<PersonMovement>();
        npcMovement.Setup(_inputAction, occupiedSpaces, positionGrid, flowSubject);
    }

    private void Update()
    {
        if (!autoMove)
        {
            return;
        }
        
        _timeTilNextMove -= Time.deltaTime;

        if (!(_timeTilNextMove < 0)) return;
        _timeTilNextMove = Random.Range(0f, 5f);

        _inputAction.ClearDirections();
        _inputAction.AddDirection((Enums.Direction)Random.Range(0, 3));
    }
}
