using UnityEngine;

public class Interactible : MonoBehaviour, IInteractible
{
    public string ReceiveInteraction()
    {
        return ("I am the princess!");
    }
}
