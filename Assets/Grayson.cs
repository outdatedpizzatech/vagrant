using UnityEngine;

public class Grayson : MonoBehaviour, IInteractible
{
    public string[] ReceiveInteraction()
    {
        return (new string[]
        {
            "I'm Grayson. As you can see, I'm pretty much just an old man.",
            "Would you like to see my collection of antique cards?"
        });
    }
}
