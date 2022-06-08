using UnityEngine;

public class Interactible : MonoBehaviour, IInteractible
{
    public string Message;
    
    public string[] ReceiveInteraction()
    {
        return (new string[] { Message });
    }
}
