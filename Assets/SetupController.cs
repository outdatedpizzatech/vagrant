using UnityEngine;

public class SetupController : MonoBehaviour, IObserver
{
    public bool DebugMode;
    
    private void Awake()
    {
        var warpingAndWiping = new Subject("warping and wiping");
        var occupiedSpacesSubject = new Subject("occupied spaces");
        var interactionSubject = new Subject("interactions from the player");
        var flowSubject = new Subject("the flow of things");
        var encounterSubject = new Subject("all things related to being in an encounter");
        
        var player = GameObject.Find("Player").GetComponent<PlayerController>();
        var klara = GameObject.Find("NPCs/Klara").GetComponent<NpcController>();
        var grayson = GameObject.Find("NPCs/Grayson").GetComponent<NpcController>();
        var keever = GameObject.Find("NPCs/Keever").GetComponent<NpcController>();
        var wiperController = GameObject.Find("Canvas/Wiper").GetComponent<WiperController>();
        var inputController = GameObject.Find("Controllers/InputController").GetComponent<InputController>();
        var positionController = GameObject.Find("Controllers/PositionController").GetComponent<PositionController>();
        var interactionController = GameObject.Find("Controllers/InteractionController").GetComponent<InteractionController>();
        var messageBoxController = GameObject.Find("WorldSpaceCanvas/MessageBox").GetComponent<MessageBoxController>();
        var inventoryBoxController = GameObject.Find("WorldSpaceCanvas/InventoryBox").GetComponent<InventoryBoxController>();
        var interactionBoxController = GameObject.Find("WorldSpaceCanvas/InteractionBox").GetComponent<InteractionBoxController>();
        var portraitBoxController = GameObject.Find("WorldSpaceCanvas/PortraitBox").GetComponent<PortraitBoxController>();
        var encounterBoxController = GameObject.Find("WorldSpaceCanvas/EncounterBox").GetComponent<EncounterBoxController>();
        var battleCommandBoxController = GameObject.Find("WorldSpaceCanvas/EncounterBox/BattleCommandBox").GetComponent<BattleCommandBoxController>();
        var flowController = GameObject.Find("Controllers/FlowController").GetComponent<FlowController>();
        var encounterController = GameObject.Find("Controllers/EncounterController").GetComponent<EncounterController>();
        var treasureA = GameObject.Find("Objects/TreasureA").GetComponent<TreasureController>();
        var treasureB = GameObject.Find("Objects/TreasureB").GetComponent<TreasureController>();
        
        wiperController.Setup(warpingAndWiping);
        player.Setup(warpingAndWiping, inputController.InputAction, occupiedSpacesSubject, positionController.PositionGrid, interactionSubject, flowSubject);
        klara.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        keever.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        grayson.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        positionController.Setup(occupiedSpacesSubject);
        interactionController.Setup(interactionSubject, positionController.PositionGrid, flowSubject, inputController.InputAction, encounterSubject);
        messageBoxController.Setup(flowSubject);
        portraitBoxController.Setup(flowSubject);
        encounterBoxController.Setup(flowSubject);
        battleCommandBoxController.Setup(flowSubject, encounterSubject);
        flowController.Setup(flowSubject);
        encounterController.Setup(encounterSubject);
        inventoryBoxController.Setup(flowSubject);
        interactionBoxController.Setup(flowSubject);
        treasureA.Setup(occupiedSpacesSubject);
        treasureB.Setup(occupiedSpacesSubject);
        
        /*
         * DEBUG
         */

        if (DebugMode)
        {
            flowSubject.AddObserver(this);
        }
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
