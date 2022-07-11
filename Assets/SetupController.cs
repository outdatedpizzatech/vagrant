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
        var contextSubject = new Subject("focusing of windows");
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
        var opponentsTransform = GameObject.Find("WorldSpaceCanvas/EncounterBox/Opponents").transform;
        var abilityAnimation = GameObject.Find("WorldSpaceCanvas/EncounterBox/AbilityAnimation").GetComponent<AbilityAnimation>();
        var battleCommandBoxController = GameObject.Find("WorldSpaceCanvas/EncounterBox/BattleCommandBox").GetComponent<BattleCommandBoxController>();
        var encounterMessageBoxController = GameObject.Find("WorldSpaceCanvas/EncounterBox/MessageBox").GetComponent<EncounterMessageBoxController>();
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
        interactionController.Setup(interactionSubject, positionController.PositionGrid, flowSubject, inputController.InputAction, encounterSubject, contextSubject);
        messageBoxController.Setup(flowSubject, contextSubject, interactionSubject);
        encounterMessageBoxController.Setup(encounterSubject);
        portraitBoxController.Setup(flowSubject);
        encounterBoxController.Setup(flowSubject);
        battleCommandBoxController.Setup(flowSubject, encounterSubject, interactionSubject);
        flowController.Setup(flowSubject, interactionSubject);
        encounterController.Setup(encounterSubject, opponentsTransform, abilityAnimation, interactionSubject);
        inventoryBoxController.Setup(flowSubject, interactionSubject, contextSubject);
        interactionBoxController.Setup(flowSubject, interactionSubject, contextSubject);
        treasureA.Setup(occupiedSpacesSubject);
        treasureB.Setup(occupiedSpacesSubject);
        
        /*
         * DEBUG
         */

        if (DebugMode)
        {
            flowSubject.AddObserver(this);
            encounterSubject.AddObserver(this);
            interactionSubject.AddObserver(this);
            contextSubject.AddObserver(this);
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
