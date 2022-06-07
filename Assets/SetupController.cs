using UnityEngine;

public class SetupController : MonoBehaviour
{
    private void Awake()
    {
        var warpingAndWiping = new Subject("warping and wiping");
        var occupiedSpacesSubject = new Subject("occupied spaces");
        
        var player = GameObject.Find("Player").GetComponent<PlayerController>();
        var princess = GameObject.Find("Princess").GetComponent<NpcController>();
        var wiperController = GameObject.Find("Canvas/Wiper").GetComponent<WiperController>();
        var inputController = GameObject.Find("InputController").GetComponent<InputController>();
        
        wiperController.Setup(warpingAndWiping);
        player.Setup(warpingAndWiping, inputController.InputDirections, occupiedSpacesSubject);
        princess.Setup(occupiedSpacesSubject);
    }
}
