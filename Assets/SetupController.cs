using UnityEngine;

public class SetupController : MonoBehaviour, IObserver
{
    private void Awake()
    {
        var warpingAndWiping = new Subject("warping and wiping");
        var occupiedSpacesSubject = new Subject("occupied spaces");
        var interactionSubject = new Subject("interactions from the player");
        var flowSubject = new Subject("the flow of things");
        
        var player = GameObject.Find("Player").GetComponent<PlayerController>();
        var klara = GameObject.Find("Klara").GetComponent<NpcController>();
        var grayson = GameObject.Find("Grayson").GetComponent<NpcController>();
        var wiperController = GameObject.Find("Canvas/Wiper").GetComponent<WiperController>();
        var inputController = GameObject.Find("InputController").GetComponent<InputController>();
        var positionController = GameObject.Find("PositionController").GetComponent<PositionController>();
        var interactionController = GameObject.Find("InteractionController").GetComponent<InteractionController>();
        var messageBoxController = GameObject.Find("WorldSpaceCanvas/MessageBox").GetComponent<MessageBoxController>();
        
        wiperController.Setup(warpingAndWiping);
        player.Setup(warpingAndWiping, inputController.InputAction, occupiedSpacesSubject, positionController.PositionGrid, interactionSubject, flowSubject);
        klara.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        grayson.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        positionController.Setup(occupiedSpacesSubject);
        interactionController.Setup(interactionSubject, positionController.PositionGrid, flowSubject);
        messageBoxController.Setup(flowSubject);
        
        /*
         * DEBUG
         */
        
        flowSubject.AddObserver(this);
    }

    public void OnNotify(SubjectMessage message)
    {
        print("EVENT: " + message);
    }

    public void OnNotify<T>(T parameters)
    {
        print("EVENT: " + parameters);
    }
}
