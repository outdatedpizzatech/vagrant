using UnityEngine;

public class SetupController : MonoBehaviour, IObserver
{
    public bool DebugMode;
    
    private void Awake()
    {
        // Message subjects
        var warpingAndWiping = new Subject("warping and wiping");
        var occupiedSpacesSubject = new Subject("occupied spaces");
        var interactionSubject = new Subject("interactions from the player");
        var flowSubject = new Subject("the flow of things");
        var windowSubject = new Subject("focusing of windows");
        var encounterSubject = new Subject("all things related to being in an encounter");

        // NPCs
        var npcs = GameObject.Find("NPCs").transform;
        var tanner = GameObject.Find("Party/Tanner").GetComponent<PlayerController>();
        var march = GameObject.Find("Party/March").GetComponent<PartyMemberController>();
        var klara = npcs.Find("Klara").GetComponent<NpcController>();
        var grayson = npcs.Find("Grayson").GetComponent<NpcController>();
        var keever = npcs.Find("Keever").GetComponent<NpcController>();
        
        // Objects
        var objects = GameObject.Find("Objects").transform;
        var treasureA = objects.Find("TreasureA").GetComponent<TreasureController>();
        var treasureB = objects.Find("TreasureB").GetComponent<TreasureController>();
        
        // Controllers
        var controllers = GameObject.Find("Controllers").transform;
        var wiperController = GameObject.Find("Canvas/Wiper").GetComponent<WiperController>();
        var inputController = controllers.Find("InputController").GetComponent<InputController>();
        var positionController = controllers.Find("PositionController").GetComponent<PositionController>();
        var interactionController = controllers.Find("InteractionController").GetComponent<InteractionController>();
        var flowController = controllers.Find("FlowController").GetComponent<FlowController>();
        var encounterController = controllers.Find("EncounterController").GetComponent<EncounterController>();
        
        // WorldSpaceCanvas
        var worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas").transform;
        var messageWindowController = worldSpaceCanvas.Find("MessageWindow").GetComponent<MessageWindowController>();
        var inventoryWindowController = worldSpaceCanvas.Find("InventoryWindow").GetComponent<InventoryWindowController>();
        var commandWindowController = worldSpaceCanvas.Find("CommandWindow").GetComponent<CommandWindowController>();
        var portraitWindowController = worldSpaceCanvas.Find("PortraitWindow").GetComponent<PortraitBoxController>();
        
        // EncounterWindow
        var encounterWindow = worldSpaceCanvas.Find("EncounterWindow");
        var encounterWindowController = worldSpaceCanvas.Find("EncounterWindow").GetComponent<EncounterWindowController>();
        var opponentsTransform = encounterWindow.Find("Opponents").transform;
        var abilityAnimation = encounterWindow.Find("AbilityAnimation").GetComponent<AbilityAnimation>();
        var damageValue = encounterWindow.Find("DamageValue").GetComponent<DamageValue>();
        var encounterCommandWindowController = encounterWindow.Find("CommandWindow").GetComponent<EncounterCommandWindowController>();
        var encounterMessageWindowController = encounterWindow.Find("MessageWindow").GetComponent<MessageWindowController>();
        
        // Setup
        wiperController.Setup(warpingAndWiping);
        tanner.Setup(warpingAndWiping, inputController.InputAction, occupiedSpacesSubject, positionController.PositionGrid, interactionSubject, flowSubject);
        march.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        klara.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        keever.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        grayson.Setup(occupiedSpacesSubject, positionController.PositionGrid, flowSubject);
        positionController.Setup(occupiedSpacesSubject);
        interactionController.Setup(interactionSubject, positionController.PositionGrid, flowSubject, inputController.InputAction, windowSubject);
        messageWindowController.Setup(flowSubject, windowSubject, interactionSubject);
        encounterMessageWindowController.Setup(encounterSubject, windowSubject, interactionSubject);
        portraitWindowController.Setup(flowSubject);
        encounterWindowController.Setup(flowSubject);
        encounterCommandWindowController.Setup(flowSubject, encounterSubject, interactionSubject);
        flowController.Setup(flowSubject, interactionSubject);
        encounterController.Setup(encounterSubject, opponentsTransform, abilityAnimation, interactionSubject, damageValue, flowSubject);
        inventoryWindowController.Setup(flowSubject, interactionSubject, windowSubject);
        commandWindowController.Setup(flowSubject, interactionSubject, windowSubject);
        treasureA.Setup(occupiedSpacesSubject);
        treasureB.Setup(occupiedSpacesSubject);
        
        // Debug
        if (!DebugMode) return;
        flowSubject.AddObserver(this);
        encounterSubject.AddObserver(this);
        interactionSubject.AddObserver(this);
        windowSubject.AddObserver(this);
    }

    public void OnNotify<T>(T parameters)
    {
        print("EVENT: " + parameters);
    }
}
