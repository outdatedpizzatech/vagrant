using UnityEngine;

public class SetupController : MonoBehaviour
{
    private void Awake()
    {
        var warpingAndWiping = new Subject("warping and wiping");
        var occupiedSpacesSubject = new Subject("occupied spaces");
        var interactionSubject = new Subject("interactions from the player");
        var flowSubject = new Subject("the flow of things");
        
        var player = GameObject.Find("Player").GetComponent<PlayerController>();
        var princess = GameObject.Find("Princess").GetComponent<NpcController>();
        var oldMan = GameObject.Find("OldMan").GetComponent<NpcController>();
        var wiperController = GameObject.Find("Canvas/Wiper").GetComponent<WiperController>();
        var inputController = GameObject.Find("InputController").GetComponent<InputController>();
        var positionController = GameObject.Find("PositionController").GetComponent<PositionController>();
        var interactionController = GameObject.Find("InteractionController").GetComponent<InteractionController>();
        var messageBoxController = GameObject.Find("Canvas/MessageBox").GetComponent<MessageBoxController>();
        
        wiperController.Setup(warpingAndWiping);
        player.Setup(warpingAndWiping, inputController.InputAction, occupiedSpacesSubject, positionController.PositionGrid, interactionSubject, flowSubject);
        princess.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        oldMan.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        positionController.Setup(occupiedSpacesSubject);
        interactionController.Setup(interactionSubject, positionController.PositionGrid, flowSubject);
        messageBoxController.Setup(flowSubject);
    }
}
