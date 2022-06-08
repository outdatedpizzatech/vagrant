using UnityEngine;

public class Interactible : MonoBehaviour, IInteractible
{
    public string Message;
    
    public string[] ReceiveInteraction(Enums.Direction direction)
    {
        return (new string[] { Message });
    }
}
