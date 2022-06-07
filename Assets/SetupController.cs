using UnityEngine;

public class SetupController : MonoBehaviour
{
    private void Start()
    {
        var subject = new Subject();
        
        var playerMovement = GameObject.Find("Hero").GetComponent<PlayerMovement>();
        var playerWarp = GameObject.Find("Hero").GetComponent<PlayerWarp>();
        var inputController = GameObject.Find("InputController").GetComponent<InputController>();
        var wiperController = GameObject.Find("Canvas/Wiper").GetComponent<WiperController>();
        
        playerMovement.Setup(inputController.InputDirections, subject);
        wiperController.Setup(subject);
        playerWarp.Setup(subject);

        subject.AddObserver(wiperController);
        subject.AddObserver(playerMovement);
    }
}
