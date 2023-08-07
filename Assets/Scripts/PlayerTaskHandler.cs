/* ds18635 2101128
 * ======================
 * This class handles almost all interactions the player has with the game. The players total resources are held here,
 * Booleans dictating whether the player is in building mode or mining mode, if there is a colonist in selected mode,
 * what task the player has set & what locations to send colonists to for certain tasks. Lastly, the placement mode UI
 * is also updated here.
 * ======================
 */
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerTaskHandler : MonoBehaviour //Player Task & Selection Handler
{
    private static List<GameObject> colonistList;
    public GameObject topGenerator;
    public GameObject editGenerator;
    public GameObject placementUI;
    public GameObject mainCamera;
    public GameObject popupCanvas;
    public GameObject riskPopup;
    public GameObject neutralPopup;
    public GameObject questUI;
    public GameObject gameoverUI;
    public Button neutralButton;
    public Button riskButton;
    public Button attachmentButton;
    public TMP_Text neutralText;
    public TMP_Text riskText;
    public bool selectMode;
    private ColonistTaskHandler colonistTaskHandler;
    public List<Tuple<int, Vector3>> colonistTaskList;
    private bool miningState;
    private bool placementState;
    private int selectedColonist;
    private SelectedState selectedState;
    private int tileType;
    public List<int> totalResources;
    public TMP_Text s, o, c, f, mk1c, mk2c, ship1, ship2, ship3;
    public TMP_Text stonex,orex,crystalx;
    public List<int> dissoResearch;
    public List<int> illegalMine;
    

    private void Start() {
        colonistTaskHandler = new ColonistTaskHandler();
        colonistTaskHandler.Start();
        colonistList = new List<GameObject>();
        illegalMine = new List<int>();        
        illegalMine.Add(10);
        illegalMine.Add(11);
        illegalMine.Add(12);
        illegalMine.Add(13);
        illegalMine.Add(14);
        illegalMine.Add(15);
        illegalMine.Add(16);
        illegalMine.Add(17);
        illegalMine.Add(18);
        illegalMine.Add(20);
        illegalMine.Add(21);
        illegalMine.Add(22);
        illegalMine.Add(23);
        illegalMine.Add(30);
        illegalMine.Add(31);
        illegalMine.Add(32);
        illegalMine.Add(33);
        illegalMine.Add(40);
        illegalMine.Add(41);
        illegalMine.Add(42);
        illegalMine.Add(43);
        NullCheck();
        ColonistUpdate(); 
        placementState = false;
        miningState = false;
    }

    public void counterrr() {
        int neutral=0;
        int risk=0;
        int attachment=0;
        int session=0;
        Debug.LogWarning(dissoResearch.Count);
        for (int i = 0; i < dissoResearch.Count; i++) {
            if (dissoResearch[i] == -1) {
                risk++;
            }
            else if (dissoResearch[i] == 0) {
                neutral++;
            }
            if (dissoResearch[i] == 1) {
                attachment++;
            }
            if (dissoResearch[i] == 2) {
                session++;
            }
        }
        Debug.LogWarning(risk);
        Debug.LogWarning(neutral);
        Debug.LogWarning(attachment);
        Debug.LogWarning(session);
    }

    private void Update() {
        ResourceUpdate();
        ColonistUpdate();
        NullCheck();
        PlayerStateCheck();
        var v = mousePos();
        for (var i = 0; i < colonistList.Count; i++) //Get the selected colonist
            if (colonistList[i].GetComponent<StateManager>().Selected) {
                selectedState = colonistList[i].GetComponent<StateManager>().selectedState;
                selectMode = true;
                selectedColonist = i;
                selectedState.MoveComplete = false;
            }

        if (Input.GetKeyDown("space") && editGenerator.GetComponent<EditHandler>().Mineactive == false
        ) //Activate & Deactivate Place Mode
        { 
            placementState = !placementState;
            BuildUI();
            if (placementState == false) {
                editGenerator.GetComponent<EditHandler>().Editactive = false;
                editGenerator.GetComponent<EditHandler>().ExitMode();
            }
        }
        if (Input.GetKeyDown("m") && editGenerator.GetComponent<EditHandler>().Editactive == false
        ) //Activate & Deactivate Mine Mode
        {
            miningState = !miningState;
            if (miningState == false) {
                editGenerator.GetComponent<EditHandler>().Mineactive = false;
                editGenerator.GetComponent<EditHandler>().ExitMode();
            }
        }

        if (placementState) //If the player is in placement mode
        {
            editGenerator.GetComponent<EditHandler>().Editactive = true;
            editGenerator.GetComponent<EditHandler>().CurrentTile(v, tileType);
        }

        if (miningState) //If the player is in mine mode
        {
            tileType = 0;
            editGenerator.GetComponent<EditHandler>().Mineactive = true;
            editGenerator.GetComponent<EditHandler>().CurrentTile(v, tileType);
        }

        if (Input.GetMouseButtonDown(0) && placementState)
            if (!EventSystem.current.IsPointerOverGameObject() && editGenerator.GetComponent<EditHandler>().validAction) {
                var location = new Tuple<int, Vector3>(tileType, v);
                colonistTaskHandler.SetColonistTask(topGenerator.GetComponent<TopographyGeneration>().topography,
                    colonistList, location);
            }

        if (Input.GetMouseButtonDown(1) && selectMode) //Leaving selected colonist state 
        {
            Debug.Log("Colonist Deselected");
            foreach (var t in colonistList) {
                t.GetComponent<StateManager>().Selected = false;
                selectMode = false;
                t.GetComponent<StateManager>().selectedState.MoveComplete = true;
            }
        }

        if (Input.GetMouseButtonDown(0) && miningState) {
            if (editGenerator.GetComponent<EditHandler>().validAction) { //Adding a mining task to the Task List
                var location = new Tuple<int, Vector3>(tileType, v);
                colonistTaskHandler.SetColonistTask(topGenerator.GetComponent<TopographyGeneration>().topography,
                    colonistList, location);
            }
        }

        if (Input.GetMouseButtonDown(0)) //Setting locations for selected colonist to walk to
            if (selectMode) {
                colonistList[selectedColonist].GetComponent<StateManager>().currentState = selectedState;
                colonistList[selectedColonist].GetComponent<StateManager>().colonistGridMovement
                    .SetTargetPosition(v);
            }
        colonistTaskHandler.Update();
    }

    public static void ColonistUpdate() {
        colonistList.Clear();
        colonistList.AddRange(GameObject.FindGameObjectsWithTag("Colonist"));
    }

    private void PlayerStateCheck() {
        var IsMouseOverGameWindow = !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y ||
                                      Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
        if (!IsMouseOverGameWindow || questUI.activeSelf) {
            foreach (var t in colonistList) {
                t.GetComponent<StateManager>().Selected = false;
                selectMode = false;
                t.GetComponent<StateManager>().selectedState.MoveComplete = true;
            }
            placementState = false;
            miningState = false;
            placementUI.SetActive(false);
            editGenerator.GetComponent<EditHandler>().ExitMode();
        }
    }

    private void ResourceUpdate() {
        if (totalResources != null) {
            s.SetText("||"+totalResources[0]);
            o.SetText("||"+totalResources[1]);
            c.SetText("||"+totalResources[2]);
            f.SetText("||"+totalResources[5]);
            mk1c.SetText("||"+totalResources[3]);
            mk2c.SetText("||"+totalResources[4]);
            ship1.SetText("||"+totalResources[7]);
            ship2.SetText("||"+totalResources[8]);
            ship3.SetText("||"+totalResources[9]); 
        }
    }

    private Vector3 mousePos() {
        var mousePos = Input.mousePosition;
        var v = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        return v;
    }

    private void BuildUI() {
        tileType = 1;
        stonex.SetText("x5 Stone");
        orex.SetText("x0 Ore");
        crystalx.SetText("x0 Crystal");
        placementUI.SetActive(placementState);
    }

    private void NullCheck() {
        if (colonistTaskList != null) return;
        colonistTaskList = new List<Tuple<int, Vector3>>();
    }

    public void Wall() {
        Debug.LogWarning("Wall Placement");
        stonex.SetText("x5 Stone");
        orex.SetText("x0 Ore");
        crystalx.SetText("x0 Crystal");
        tileType = 1;
    }

    public void Research() {
        Debug.LogWarning("Research Placement");
        stonex.SetText("x100 Stone");
        orex.SetText("x100 Ore");
        crystalx.SetText("x25 Crystal");
        tileType = 2;
    }

    public void Food() {
        Debug.LogWarning("Food Placement");
        stonex.SetText("x150 Stone");
        orex.SetText("x50 Ore");
        crystalx.SetText("x25 Crystal");
        tileType = 3;
    }

    public void ShipYard() {
        Debug.LogWarning("Ship yard Placement");
        stonex.SetText("x100 Stone");
        orex.SetText("x150 Ore");
        crystalx.SetText("x25 Crystal");
        tileType = 4;
    }

    public void HUB() {
        Debug.LogWarning("HUB Placement");
        stonex.SetText("x200 Stone");
        orex.SetText("x150 Ore");
        crystalx.SetText("x50 Crystal");
        tileType = 5;
    }

    public void TaskAvailable(Tuple<int, Vector3> task) {
        colonistTaskHandler.SetColonistTask(topGenerator.GetComponent<TopographyGeneration>().topography, colonistList,
            task);
    }

    public void MissionAvailable(Tuple<int, Vector3> task, GameObject colonist) {
        colonistTaskHandler.setColonistMission(topGenerator.GetComponent<TopographyGeneration>().topography, colonist,
            task);
    }

    public void EmbarkTask() {
        ColonistUpdate();
        var scale = GetComponent<TaskMenu>().scale;
        var taskIndex = 0;
        Tuple<int, Vector3> task;
        switch (scale) {
            
            case 1:
                taskIndex = 6;
                break;
            case 2:
                taskIndex = 7;
                break;
            case 3:
                taskIndex = 8;
                break;
            case 4:
                taskIndex = 9;
                break;
            case 5:
                taskIndex = 10;
                break;
        }
        var colonist = new GameObject();
        for (var i = 0; i < colonistList.Count; i++)
            if (colonistList[i].GetComponent<StateManager>().colName == mainCamera.GetComponent<TaskMenu>()
                .colonistSelectMT.options[mainCamera.GetComponent<TaskMenu>().colonistSelectMT.value].text)
                colonist = colonistList[i];
        for (var i = 0; i < 64; i++)
        for (var j = 0; j < 64; j++)
            if (topGenerator.GetComponent<TopographyGeneration>().topography[i, j] == 10) {
                task = new Tuple<int, Vector3>(taskIndex, new Vector3(i, j));
                colonistTaskHandler.setColonistMission(topGenerator.GetComponent<TopographyGeneration>().topography,
                    colonist, task);
            }
    }

    public void CraftingTask() {
        ColonistUpdate();
        var scale = GetComponent<TaskMenu>().scale;
        var taskIndex = 0;
        Tuple<int, Vector3> task;
        switch (scale) {
            case 6:
                taskIndex = 12;
                break;
            case 7:
                taskIndex = 13;
                break;
            case 8:
                taskIndex = 14;
                break;
        }
        var colonist = new GameObject();
        for (var i = 0; i < colonistList.Count; i++)
            if (colonistList[i].GetComponent<StateManager>().currentState != colonistList[i].GetComponent<StateManager>().busyState)
                colonist = colonistList[i];
        bool research = false;
        for (var i = 0; i < 64; i++)
        for (var j = 0; j < 64; j++)
            if (topGenerator.GetComponent<TopographyGeneration>().topography[i, j] == 30) {
                research = true;
            }
        if (research) {
            for (var i = 0; i < 64; i++)
            for (var j = 0; j < 64; j++)
                if (topGenerator.GetComponent<TopographyGeneration>().topography[i, j] == 40) {
                    task = new Tuple<int, Vector3>(taskIndex, new Vector3(i, j));
                    colonistTaskHandler.setColonistMission(topGenerator.GetComponent<TopographyGeneration>().topography,
                        colonist, task);
                    return; 
                }
        }
    }

    public void FarmingTask() {
        ColonistUpdate();
        Tuple<int, Vector3> task;
        var colonist = new GameObject();
        for (var i = 0; i < colonistList.Count; i++)
            if (colonistList[i].GetComponent<StateManager>().colName == mainCamera.GetComponent<TaskMenu>()
                .colonistSelectFT.options[mainCamera.GetComponent<TaskMenu>().colonistSelectFT.value].text)
                colonist = colonistList[i];
        for (var i = 0; i < 64; i++)
        for (var j = 0; j < 64; j++)
            if (topGenerator.GetComponent<TopographyGeneration>().topography[i, j] == 20) {
                task = new Tuple<int, Vector3>(11, new Vector3(i, j));
                colonistTaskHandler.setColonistMission(topGenerator.GetComponent<TopographyGeneration>().topography,
                    colonist, task);
                return;
            }
    }

    public bool ResourceCheck(List<int> requiredResources) {
        for (int i = 0; i < requiredResources.Count; i++) {
            if (requiredResources[i] != 0 && totalResources[i] < requiredResources[i]) {
                Debug.LogWarning(totalResources[i]+ " " + requiredResources[i]);
                return false;
            }
        }
        return true;
    }

    public void MissionOutcome(List<int> resource) {
        /*
         [0]Stone
         [1]Ore
         [2]Crystals
         [3]MK1
         [4]MK2
         [5]Food
         [6]Colonist
         [7]Ship1
         [8]Ship2
         [9]Ship3
        */
        //TODO Check if a colonist is on a tile of a certain value
        if (resource == null) return;
        if (resource[6] > 0) {
            mainCamera.GetComponent<MainHandler>().loadHandler.GetComponent<LoadHandler>().SpawnColonists(resource[6], new Vector3(32,32,0));
        }
        for (var i = 0; i < resource.Count; i++) {
            var newTotal = totalResources[i] += resource[i];
            totalResources[i] = newTotal;
        }
        for (int i = 0; i < colonistList.Count; i++) {
            int xPos = (int) colonistList[i].GetComponent<Rigidbody2D>().position.x;
            int yPos = (int) colonistList[i].GetComponent<Rigidbody2D>().position.y;
            if (illegalMine.Contains(topGenerator.GetComponent<TopographyGeneration>().topography[xPos,yPos])) {
                Debug.LogWarning("Colonist in Illegal location");
                 colonistList[i].GetComponent<Rigidbody2D>().position = new Vector3(32,32);
            }
            /*if (topGenerator.GetComponent<TopographyGeneration>().basemap[xPos,yPos] == 1) {
                Debug.LogWarning("Colonist in Illegal location");
                colonistList[i].GetComponent<Rigidbody2D>().position = new Vector3(32,32);
            }*/
        }
        /*Debug.LogWarning(totalResources[0] + "x Stone");
        Debug.LogWarning(totalResources[1] + "x Ore");
        Debug.LogWarning(totalResources[2] + "x Crystals");
        Debug.LogWarning(totalResources[3] + "x MK1");
        Debug.LogWarning(totalResources[4] + "x MK2");
        Debug.LogWarning(totalResources[5] + "x Food");
        Debug.LogWarning(totalResources[6] + "x Colonists");
        Debug.LogWarning(totalResources[7] + "x Ship MK1");
        Debug.LogWarning(totalResources[8] + "x Ship MK2");
        Debug.LogWarning(totalResources[9] + "x Ship MK3");*/
    }

    public void GameOver() {
        gameoverUI.SetActive(true);
    }
}
/*
 1) Get the mouse click coordinates,                DONE
 2) Convert the screen click to world point,        DONE
 3) Check the tilemaps value,                       DONE
 4) Change the tilemaps value to a different tile,  DONE
 5) Update the tilemaps value,                      DONE
 ------------------------------------------------------->
 1) Set a task for Right Click AND Left click colonist == DRAFTED state                                         DONE
 2) RIGHT CLICK = Check what the value of the position is > 0 where 0 is unoccupied. THEN check the tilemaps    DONE
 neighbours for the value 0, and the "first" 0 is set as the location to path                                   DONE
 3) Set a state for after going to a task == BUSY state --> Timer for doing a task at a set location            DONE
             //Add to a workingTaskList Containing: --> Send a new task to TASK HANDLER class
            //The colonist                                  -->New class to handle what type of task                YEs
            //int Index: The type of task to do             -->AKA MINING                                           YEs
            //Vector3 Pos: The position of the tile         -->Vector3 will be checked for neighbours to walk to    YEs
            //--> TASK HANDLER will have a list of the things sent over, and if a task has not been completed it is re-added to the task list
            //Handle the relationship from MoveState to BusyState if a workingTask
            //Set to busy state with:
            //Timer to check how long the task should be done
            //Timer scaling with trait value
            //-->Need to be able to check a value after the timer to confirm with the TaskHandler class that it can be removed from the list?
            //-->Only way to check if the task is disrupted is by a state change before the timer is over? A bool to see if the timer is complete or not
            //Checker to see if the task has been completed
            //THEN RemoveTile
*/