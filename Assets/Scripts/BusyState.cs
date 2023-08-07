/* ds18635 2101128
 * ======================
 * This script is one of the States in the state machine & is transitioned through the MoveState. This state assigns the
 * colonist different tasks according to an index value governed by the PlayerTaskHandler. Mission tasks in particular
 * have their probability and scenarios calculated here to be presented to the player. Further, rewards or losses from
 * tasks are calculated here to handle the player’s resources. 
 * ======================
 */

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BusyState : State {
    public string colname;
    public IdleState idleState;
    public GameObject topographyGeneration;
    public GameObject playerTaskHandler;
    public GameObject popUpCanvas;
    public GameObject popUp1;
    public GameObject popUp2;
    public int index;
    public bool taskComplete;
    public bool taskSet;
    public Vector3 taskTarget;
    private Button attachmentButton;
    private ColonistGridMovement colonistGridMovement;
    private int consequence;
    private bool missionSuccess;
    private Button neutralButton;
    private TMP_Text neutralScenario;
    private string outcome;
    private List<int> resource;
    private List<int> resourceRequired;
    private Button riskButton;
    private TMP_Text riskScenario;
    private string scenario;

    private void Start() {
        ClearResources();
        colonistGridMovement = GetComponentInParent<ColonistGridMovement>();
        topographyGeneration = GameObject.FindWithTag("Topography");
        playerTaskHandler = GameObject.FindWithTag("MainCamera");
        popUpCanvas = playerTaskHandler.GetComponent<PlayerTaskHandler>().popupCanvas;
        popUp1 = playerTaskHandler.GetComponent<PlayerTaskHandler>().neutralPopup;
        popUp2 = playerTaskHandler.GetComponent<PlayerTaskHandler>().riskPopup;
        neutralButton = playerTaskHandler.GetComponent<PlayerTaskHandler>().neutralButton;
        attachmentButton = playerTaskHandler.GetComponent<PlayerTaskHandler>().attachmentButton;
        riskButton = playerTaskHandler.GetComponent<PlayerTaskHandler>().riskButton;
        neutralScenario = playerTaskHandler.GetComponent<PlayerTaskHandler>().neutralText;
        riskScenario = playerTaskHandler.GetComponent<PlayerTaskHandler>().riskText;
    }

    public override State RunCurrentState() {
        if (!taskComplete && !taskSet) {
            ClearResources();
            switch (index) {
                case 0:
                    var multiplier = 9f * (colonistGridMovement.traits[0] / 10f);
                    TaskTimer.Create(Mining, 9f - multiplier, colname + " Task");
                    taskSet = true;
                    break;
                case 11:
                    multiplier = 9f * (colonistGridMovement.traits[3] / 10f);
                    TaskTimer.Create(Farming, 30f - (multiplier / 10f), colname + " Task");
                    taskSet = true;
                    break;
                case 1:
                    multiplier = 9f * (colonistGridMovement.traits[1] / 10f);
                    TaskTimer.Create(Building, 9f - multiplier, colname + " Task");
                    taskSet = true;
                    break;
                case 2:
                    multiplier = 9f * (colonistGridMovement.traits[1] / 10f);
                    TaskTimer.Create(Building, 15f - multiplier, colname + " Task");
                    taskSet = true;
                    break;
                case 3:
                    multiplier = 9f * (colonistGridMovement.traits[1] / 10f);
                    TaskTimer.Create(Building, 15f - multiplier, colname + " Task");
                    taskSet = true;

                    break;
                case 4:
                    multiplier = 9f * (colonistGridMovement.traits[1] / 10f);
                    TaskTimer.Create(Building, 15f - multiplier, colname + " Task");
                    taskSet = true;
                    break;
                case 5:
                    multiplier = 9f * (colonistGridMovement.traits[1] / 10f);
                    TaskTimer.Create(Building, 20f - multiplier, colname + " Task");
                    taskSet = true;
                    break;
                case 6:
                    resource[7] = -1;
                    resource[1] = -50;
                    resource[2] = -50;
                    resource[5] = -25;
                    break;
                case 7:
                    resource[7] = -1;
                    resource[1] = -75;
                    resource[2] = -75;
                    resource[5] = -75;
                    break;
                case 8:
                    resource[8] = -1;
                    resource[1] = -125;
                    resource[2] = -125;
                    resource[5] = -125;
                    break;
                case 9:
                    resource[8] = -1;
                    resource[1] = -175;
                    resource[2] = -175;
                    resource[5] = -175;
                    break;
                case 10:
                    resource[9] = -1;
                    resource[1] = -500;
                    resource[2] = -500;
                    resource[5] = -500;
                    break;
                case 12:
                    resource[0] = -25;
                    resource[1] = -100;
                    resource[2] = -50;
                    break;
                case 13:

                    resource[3] = -2;
                    resource[1] = -150;
                    resource[7] = -1;
                    break;
                case 14:
                    resource[4] = -4;
                    resource[1] = -300;
                    resource[8] = -2;
                    break;
            }
            if (!taskSet) {
                for (int i = 0; i < 10; i++) {
                    int r = resource[i];
                    resourceRequired[i] = -r;
                }
            }
            if (playerTaskHandler.GetComponent<PlayerTaskHandler>().ResourceCheck(resourceRequired) && !taskSet) {
                Debug.LogWarning("Setting Flying & Crafting Tasks");
                switch (index) {
                    case 6:
                        var multiplier = 9f * (colonistGridMovement.traits[2] / 10f);
                        colonistGridMovement.GetComponent<SpriteRenderer>().enabled = false;
                        TaskTimer.Create(MissionTarget, 20f - multiplier, "Flying Task");
                        break;
                    case 7:
                        multiplier = 9f * (colonistGridMovement.traits[2] / 10f);
                        colonistGridMovement.GetComponent<SpriteRenderer>().enabled = false;
                        TaskTimer.Create(MissionTarget, 30f - multiplier, "Flying Task");
                        break;
                    case 8:
                        multiplier = 9f * (colonistGridMovement.traits[2] / 10f);
                        colonistGridMovement.GetComponent<SpriteRenderer>().enabled = false;
                        TaskTimer.Create(MissionTarget, 40f - multiplier, "Flying Task");
                        break;
                    case 9:
                        multiplier = 9f * (colonistGridMovement.traits[2] / 10f);
                        colonistGridMovement.GetComponent<SpriteRenderer>().enabled = false;
                        TaskTimer.Create(MissionTarget, 50f - multiplier, "Flying Task");
                        break;
                    case 10:
                        playerTaskHandler.GetComponent<PlayerTaskHandler>().GameOver();
                        break;
                    case 12:
                        multiplier = 9f * (colonistGridMovement.traits[1] / 10f);
                        TaskTimer.Create(Crafting, 10f * (multiplier / 15), colname + " Task");
                        break;
                    case 13:
                        multiplier = 9f * (colonistGridMovement.traits[1] / 10f);
                        TaskTimer.Create(Crafting, 20f * (multiplier / 15), colname + "Task");
                        break;
                    case 14:
                        multiplier = 9f * (colonistGridMovement.traits[1] / 10f);
                        TaskTimer.Create(Crafting, 40f * (multiplier / 15), colname + " Task");
                        break;
                }
                playerTaskHandler.GetComponent<PlayerTaskHandler>().MissionOutcome(resource);
                ClearResources();
                taskSet = true;
            }
        }
        if (!taskSet) {
            return idleState;
        }
        if (taskComplete) {
            taskSet = false;
            taskComplete = false;
            return idleState;
        }
        return this;
    }

    private void ClearResources() {
        resource = new List<int>();
        resourceRequired = new List<int>();
        for (var i = 0; i < 10; i++) resource.Add(0);
        for (var i = 0; i < 10; i++) resourceRequired.Add(0);
    }

    private void Progression(int v) {
        var trait = colonistGridMovement.traits[v];
        var progress = colonistGridMovement.progression[v];
        if (trait < 9) {
            colonistGridMovement.progression[v] = progress + 1;
            if (colonistGridMovement.progression[v] == 5) //Can Simplify with ||
            {
                colonistGridMovement.traits[v] = trait + 1;
                Debug.LogWarning("Trait Progression: " + colonistGridMovement.progression[1] + " -- Trait Value: " +
                                 colonistGridMovement.traits[1]);
            }

            if (colonistGridMovement.progression[v] == 15) {
                colonistGridMovement.traits[v] = trait + 1;
                Debug.LogWarning("Trait Progression: " + colonistGridMovement.progression[1] + " -- Trait Value: " +
                                 colonistGridMovement.traits[1]);
            }

            if (colonistGridMovement.progression[v] == 30) {
                colonistGridMovement.traits[v] = trait + 1;
                Debug.LogWarning("Trait Progression: " + colonistGridMovement.progression[1] + " -- Trait Value: " +
                                 colonistGridMovement.traits[1]);
            }

            if (colonistGridMovement.progression[v] == 50) {
                colonistGridMovement.traits[v] = trait + 1;
                Debug.LogWarning("Trait Progression: " + colonistGridMovement.progression[1] + " -- Trait Value: " +
                                 colonistGridMovement.traits[1]);
            }

            if (colonistGridMovement.progression[v] == 80) {
                colonistGridMovement.traits[v] = trait + 1;
                Debug.LogWarning("Trait Progression: " + colonistGridMovement.progression[1] + " -- Trait Value: " +
                                 colonistGridMovement.traits[1]);
            }

            if (colonistGridMovement.progression[v] == 115) {
                colonistGridMovement.traits[v] = trait + 1;
                Debug.LogWarning("Trait Progression: " + colonistGridMovement.progression[1] + " -- Trait Value: " +
                                 colonistGridMovement.traits[1]);
            }

            if (colonistGridMovement.progression[v] == 155) {
                colonistGridMovement.traits[v] = trait + 1;
                Debug.LogWarning("Trait Progression: " + colonistGridMovement.progression[1] + " -- Trait Value: " +
                                 colonistGridMovement.traits[1]);
            }

            if (colonistGridMovement.progression[v] == 200) {
                colonistGridMovement.traits[v] = trait + 1;
                Debug.LogWarning("Trait Progression: " + colonistGridMovement.progression[1] + " -- Trait Value: " +
                                 colonistGridMovement.traits[1]);
            }
        }
    }

    private void Building() {
        ClearResources();
        switch (index) {
            case 1: //Wall
                resource[0] = -5;
                break;
            case 2: //Research
                resource[0] = -100;
                resource[1] = -100;
                resource[2] = -25;
                break;
            case 3: //Food
                resource[0] = -150;
                resource[1] = -50;
                resource[2] = -25;
                break;
            case 4: //Shipyard
                resource[0] = -100;
                resource[1] = -150;
                resource[2] = -25;
                break;
            case 5: //Hub
                resource[0] = -200;
                resource[1] = -150;
                resource[2] = -50;
                break;
        }
        for (int i = 0; i < 10; i++) {
            int r = resource[i];
            resourceRequired[i] = -r;
        }
        if (playerTaskHandler.GetComponent<PlayerTaskHandler>().ResourceCheck(resourceRequired)) {
            topographyGeneration.GetComponent<TopographyGeneration>().SetTile(taskTarget, index);
            Progression(1);
            playerTaskHandler.GetComponent<PlayerTaskHandler>().MissionOutcome(resource);
        }
        taskComplete = true;
    }

    private void Mining() {
        ClearResources();
        var top = topographyGeneration.GetComponent<TopographyGeneration>().topography;
        if (top[(int) taskTarget.x, (int) taskTarget.y] == 1) {
            resource[0] = 5;
        }

        if (top[(int) taskTarget.x, (int) taskTarget.y] == 2) {
            resource[1] = 5;
        }

        if (top[(int) taskTarget.x, (int) taskTarget.y] == 4) {
            resource[2] = 10;
        }

        topographyGeneration.GetComponent<TopographyGeneration>().RemoveTile(taskTarget);
        Progression(0);
        playerTaskHandler.GetComponent<PlayerTaskHandler>().MissionOutcome(resource);
        taskComplete = true;
    }

    private void Farming() {
        ClearResources();
        resource[5] = 50;
        Progression(3);
        playerTaskHandler.GetComponent<PlayerTaskHandler>().MissionOutcome(resource);
        taskComplete = true;
    }

    private void Crafting() {
        ClearResources();
        Progression(1);
        if (index == 12) {
            resource[7] = 1;
        }
        else if (index == 13) {
            resource[8] = 1;
        }
        else {
            resource[9] = 1;
        }

        playerTaskHandler.GetComponent<PlayerTaskHandler>().MissionOutcome(resource);
        taskComplete = true;
    }

    private void MissionTarget() {
        Debug.LogWarning("Mission Reached");
        Progression(2);
        if (colonistGridMovement.traits[2] < 9) {
            var trait = colonistGridMovement.traits[2];
            colonistGridMovement.traits[2] = trait + 1;
        }

        neutralButton.onClick.AddListener(Neutral);
        riskButton.onClick.AddListener(Risk);
        attachmentButton.onClick.AddListener(Attachment);
        switch (index) {
            case 6:
                SmallAsteroidBank();
                popUpCanvas.SetActive(true);
                popUp1.SetActive(true);
                break;
            case 7:
                ShipWreckBank();
                break;
            case 8:
                LargeAsteroidBank();
                popUpCanvas.SetActive(true);
                popUp2.SetActive(true);
                break;
            case 9:
                CubeBank();
                popUpCanvas.SetActive(true);
                popUp2.SetActive(true);
                break;
        }
        neutralScenario.SetText(scenario);
        riskScenario.SetText(scenario);
    }

    private void MissionComplete() {
        Progression(2);
        Debug.LogWarning("Back!");
        if (colonistGridMovement != null) {
            colonistGridMovement.GetComponent<SpriteRenderer>().enabled = true;
            playerTaskHandler.GetComponent<PlayerTaskHandler>().MissionOutcome(resource);
            taskComplete = true;   
        }
        if (consequence == 2  && !missionSuccess) {
            colonistGridMovement.GetComponent<ColonistGridMovement>().Dead();
        }
    }
    private void Attachment() {
        Debug.LogWarning("Attachment Button Click");
        ClearResources();
        switch (index) {
            case 8:
            case 9:
                resource[8] = 1;
                break;
            case 7:
            case 6:
                resource[7] = 1;
                break;
        }

        playerTaskHandler.GetComponent<PlayerTaskHandler>().dissoResearch.Add(1);
        var multiplier = 9f * (colonistGridMovement.traits[2] / 10f);
        TaskTimer.Create(MissionComplete, 5f - multiplier, "Flying Back Task");
        neutralButton.onClick.RemoveAllListeners();
        attachmentButton.onClick.RemoveAllListeners();
        riskButton.onClick.RemoveAllListeners();
        ClosePopUp();
    }

    private void Risk() {
        Debug.LogWarning("Risk Button Click");
        playerTaskHandler.GetComponent<PlayerTaskHandler>().dissoResearch.Add(-1);
        riskScenario.SetText(outcome);
        var multiplier = 9f * (colonistGridMovement.traits[2] / 10f);
        if (missionSuccess) {
            TaskTimer.Create(MissionComplete, 5f - multiplier, "Flying Back Task");
        }
        else { //FAIL
            switch (consequence) {
                case 1: //Broken arm outcome 
                    var current = colonistGridMovement.traits[0];
                    colonistGridMovement.traits[0] = current - 1;
                    TaskTimer.Create(MissionComplete, 5f - multiplier, "Flying Back Task");
                    break;
                case 2: //Death 
                    TaskTimer.Create(MissionComplete, 5f - multiplier, "Flying Back Task");
                    break;
                case 3: //Death + Ship lost
                    Debug.LogWarning("Everything lost");
                    TaskTimer.Create(MissionComplete, 7f, "Total Mission Failure");
                    colonistGridMovement.GetComponent<ColonistGridMovement>().Dead();
                    break;
                case 4: //Trait shuffle
                    var traits = new List<int>();
                    for (var i = 0; i < 5; i++) {
                        var number = Random.Range(0, 9);
                        traits.Add(number);
                    }

                    for (var i = 0; i < traits.Count; i++) colonistGridMovement.traits[i] = traits[i];
                    TaskTimer.Create(MissionComplete, 5f - multiplier, "Flying Back Task");
                    break;
            }
        }

        neutralButton.onClick.RemoveAllListeners();
        attachmentButton.onClick.RemoveAllListeners();
        riskButton.onClick.RemoveAllListeners();
        TaskTimer.Create(ClosePopUp, 6.5f, "Close Menu");
    }

    private void ClosePopUp() {
        popUp1.SetActive(false);
        popUp2.SetActive(false);
        popUpCanvas.SetActive(false);
    }

    public void Neutral() {
        Debug.LogWarning("Mission Return");
        var multiplier = 9f * (colonistGridMovement.traits[2] / 10f);
        playerTaskHandler.GetComponent<PlayerTaskHandler>().dissoResearch.Add(0);
        TaskTimer.Create(MissionComplete, 5f - multiplier, "Flying Task");
        popUp1.SetActive(false);
        popUp2.SetActive(false);
        popUpCanvas.SetActive(false);
        neutralButton.onClick.RemoveAllListeners();
        attachmentButton.onClick.RemoveAllListeners();
        riskButton.onClick.RemoveAllListeners();
    }

    private void SmallAsteroidBank() {
        var chosenScenario = Random.Range(0, 11);
        var secondChoice = Random.Range(1, 11); //Type
            var thirdChoice = Random.Range(1, 7); //Amount
            if (thirdChoice <= 3) { //Min
                resource[1] = 75;
                resource[2] = 75;
            }
            else if (secondChoice == 4 || secondChoice == 5) { //Mid
                resource[1] = 100;
                resource[2] = 50;
            }
            else{ 
                resource[1] = 40;
                resource[2] = 40;
            }
            scenario = "Your colonist was able to find " + resource[2] + "x Crystals" + "\n & also found" + resource[1] + "x Ore";
            resource[7] = 1;
    }

    private void ShipWreckBank() {
        var wreckType = Random.Range(0, 11);
        if (wreckType <= 4) { //Search
            missionSuccess = true;
            var searchResult = Random.Range(1, 6);
            if (searchResult >= 3) {
                resource[2] = 100;
                resource[5] = 75;
                scenario = "Your colonist comes upon a cargo ship!" +
                           "\nYour colonist found x100 Crystals & 75 Food...";
            }
            else {
                resource[3] = 1;
                scenario = "Your colonist comes upon a cargo ship!" + "\nYour colonist found a MK1 Ship core!";
            }

            popUpCanvas.SetActive(true);
            popUp1.SetActive(true);
            resource[7] = 1;
        }
        else if (wreckType <= 8) { //Colonist Risk
            var riskType = Random.Range(1, 11);
            if (riskType <= 5) {
                scenario = "Your colonist can see another colonist in combat with an Alien! Risk themself to help?";
                var combatResult = Random.Range(0, 11);
                if (colonistGridMovement.traits[0] >= combatResult) {
                    outcome = "Your colonist saved the other! Your colony has grown...";
                    missionSuccess = true;
                    resource[6] = 1;
                    resource[7] = 1;
                }
                else {
                    outcome = "Your colonist failed! Your colony has grown smaller...";
                    missionSuccess = false;
                    consequence = 3;
                    resource[6] = -1;
                    resource[7] = -1;
                }
            }
            else {
                missionSuccess = false;
                consequence = 2;
                scenario =
                    "Your colonist hears an unknown number of colonists behind an airlock! Sacrifice your colonist to save them?";
                var number = Random.Range(1, 11);
                if (number >= 7) {
                    resource[6] = 1;
                    outcome =
                        "Your colonist faded away into the stars... Your colonist saved 1 others... \nBut at what cost?";
                }
                else {
                    resource[6] = 2;
                    outcome =
                        "Your colonist faded away into the stars... Your colonist saved 2 others... \nBut at what cost?";
                }

                resource[7] = 1;
            }

            popUpCanvas.SetActive(true);
            popUp2.SetActive(true);
        }
        else {
            resource[6] = 1;
            resource[7] = 1;
            missionSuccess = true;
            scenario = "Your colonist found another stranded colonist!";
            popUpCanvas.SetActive(true);
            popUp1.SetActive(true);
        }
    }

    private void LargeAsteroidBank() {
        int resourceType;
        string resourceName;
        var chosenScenario = Random.Range(0, 21);
        if (chosenScenario == 20 || chosenScenario == 19 || chosenScenario == 18) {
            scenario = "Aliens! You're colonist has encountered an alien...";
            var combatResult = Random.Range(1, 11);
            if (combatResult > colonistGridMovement.traits[0]) {
                consequence = 3;
                missionSuccess = false;
                outcome = "Your colonist was brutalised and the alien looted all that remained...";
                resource[8] = -1;
            }
            else {
                missionSuccess = true;
                outcome = "Your colonist towers over the aliens terminated body looting 100x Crystals and Ore";
                resource[1] = 100;
                resource[2] = 100;
                resource[8] = 1;
            }
        }
        else if (chosenScenario <= colonistGridMovement.traits[0] + 6) {
            missionSuccess = true;
            scenario = "Your colonist has stumbled upon an unstable cave...";
            var secondChoice = Random.Range(1, 11);
            if (secondChoice <= 3) {
                resourceType = 0;
                resourceName = "Stone!";
            }
            else if (secondChoice <= 7) {
                resourceType = 1;
                resourceName = "Ore!";
            }
            else {
                resourceType = 2;
                resourceName = "Crystals!";
            }

            resource[resourceType] = 45 * colonistGridMovement.traits[0] / (resourceType + 1);
            resource[8] = 1;
            outcome = "Your colonist was able to find " + resource[resourceType] + "x " + resourceName;
        }
        else {
            missionSuccess = false;
            scenario = "Your colonist has stumbled upon an unstable cave...";
            var failedOutcome = Random.Range(1, 21);
            if (failedOutcome <= 15) {
                resource[8] = 1;
                consequence = 1;
                outcome =
                    "Your colonist unfortunately broke his arm! Unable to carry anything, he returns  to the ship empty handed...";
            }
            else if (failedOutcome <= 19) {
                consequence = 2;
                outcome =
                    "Your colonist had a airlock malfunction! The ship and collected resources will return but unfortunately your colonist will not make it back...";
                resource[0] = 100;
                resource[1] = 200;
                resource[2] = 75;
                resource[6] = -1;
                resource[8] = -1;
            }
            else {
                consequence = 3;
                outcome =
                    "The unstable cave collapsed! losing your ship, colonists and resources...";
                resource[6] = -1;
                resource[8] = -1;
            }
        }
    }

    private void CubeBank() {
        resource[8] = 1;
        var scenarioType = Random.Range(1, 7);
        if (scenarioType <= 3) {
            scenario = "A mysterious cloud lies in front of you...";
            var result = Random.Range(1, 11);
            if (result <= 5) {
                //Trait shuffle
                missionSuccess = false;
                consequence = 4;
                outcome = "Your colonist feels very dizzy... Traits have been shuffled...";
            }
            else {
                //Super intelligence
                missionSuccess = true;
                outcome = "Your colonists brain is PuLsAtInG! Super Intelligence acquired.";
                var current = colonistGridMovement.traits[2];
                colonistGridMovement.traits[2] = current + 4;
            }
        }
        else {
            scenario = "Your colonist steps onto a glowing blue floor...";
            var result = Random.Range(1, 11);
            if (result <= 5) {
                outcome = "Your colonist found a MK2 Core!";
                resource[4] = 1;
            }
            else if (result <= 7) {
                outcome = "Your colonist found some spicy fruit! x155 Food gained!";
                resource[5] = 155;
            }
            else {
                missionSuccess = false;
                consequence = 1;
                outcome = "Your colonist was attacked by glowing vines and got injured!";
            }
        }
    }

    public void SetName(String s) {
        colname = s;
    }
}