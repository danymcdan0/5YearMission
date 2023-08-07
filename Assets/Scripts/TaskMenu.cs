/* ds18635 2101128
 * ======================
 * This class would handle the main UI of the game, dictating which game object would be visible at a given time.
 * Further the class would handle any colonist information changes made by the player.
 * ======================
 */
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskMenu : MonoBehaviour {
    public GameObject taskUI;
    public GameObject resourceUI;
    public TMP_Dropdown colonistSelectCT;
    public TMP_Dropdown colonistColorSelect;
    public TMP_Dropdown colonistSelectMT;
    public TMP_Dropdown colonistSelectFT;
    public GameObject colonistTab;
    public GameObject missionTab;
    public GameObject farmingTab;
    public GameObject craftingTab;
    public GameObject topography;
    public GameObject col;
    public List<GameObject> colonistList;
    public Image colonistImage;
    public TMP_Text ore;
    public TMP_Text food;
    public TMP_Text crystal;
    public TMP_Text T1;
    public TMP_Text T2;
    public TMP_Text T3;
    public TMP_Text T4;
    public TMP_Text T5;
    public TMP_Text totalFarms;
    public TMP_Text openFarms;
    public TMP_Text missionName;
    public TMP_InputField rename;
    public Image resource1;
    public Image resource2;
    public Image resource3;
    public Image selectedShip;
    public Image missionShip;
    public TMP_Text r1;
    public TMP_Text r2;
    public TMP_Text r3;
    private readonly Color32 ship1 = new Color32(161,70,96, 255);
    private readonly Color32 ship2 = new Color32(25,100,142,255);
    private readonly Color32 ship3 = new Color32(56,33,202,255);
    private readonly Color32 stoneColor = new Color32(101,101,101,255);
    private readonly Color32 oreColor = new Color32(120,94,94,255);
    private readonly Color32 crystalColor = new Color32(113,43,159,255);
    private readonly Color32 MK1Color = new Color32(43,75,43,255);
    private readonly Color32 MK2Color = new Color32(0,126,255,255);
    public int scale;
    private void Start() {
        taskUI.SetActive(false);
        colonistList = new List<GameObject>();
        ColonistUpdate();
    } 

    private void Update() {
        if (Input.GetKeyDown("q")) {
            ColonistUpdate();
            taskUI.SetActive(!taskUI.activeSelf);
            resourceUI.SetActive(!taskUI.activeSelf);
            TabReset();
        }
        MissionTextUpdate();
    }

    private void ColonistUpdate() {
        colonistList.Clear();
        colonistList.AddRange(GameObject.FindGameObjectsWithTag("Colonist"));
        var colonistOptions = new List<string>();
        var farmingOptions = new List<string>();
        for (var i = 0; i < colonistList.Count; i++)
            colonistOptions.Add(colonistList[i].GetComponent<StateManager>().colName);
        for (var i = 0; i < colonistList.Count; i++)
            if (colonistList[i].GetComponent<StateManager>().currentState != colonistList[i].GetComponent<StateManager>().busyState) {
                farmingOptions.Add(colonistList[i].GetComponent<StateManager>().colName);
            }
        colonistSelectCT.ClearOptions();
        colonistSelectCT.AddOptions(colonistOptions);
        colonistSelectMT.ClearOptions();
        colonistSelectMT.AddOptions(colonistOptions);
        colonistSelectFT.ClearOptions();
        colonistSelectFT.AddOptions(farmingOptions);
    }

    public void ClickSmallAsteroid() {
        scale = 1;
        missionName.SetText("Nearby Asteroid");
    }

    public void ClickShipwreck() {
        scale = 2;
        missionName.SetText("Ship wreck!");
    }

    public void ClickBigAsteroid() {
        scale = 3;
        missionName.SetText("Abandoned moon...");
    }

    public void Cube() {
        scale = 4;
        missionName.SetText("Q█☐Be_");
    }

    public void FinalMission() {
        scale = 5;
        missionName.SetText("FINAL MISSION");
    }

    public void BasicShip() {
        scale = 6;
        r1.SetText("100x Ore");
        r2.SetText("50x Crystals") ;
        r3.SetText("25x Stone");
        selectedShip.color = ship1;
        resource1.color= oreColor;
        resource2.color = crystalColor;
        resource3.color= stoneColor;
    }

    public void MK1Ship() {
        scale = 7;
        r1.SetText("1x Cargo Ship");
        r2.SetText("2x MK1 Core") ;
        r3.SetText("150x Ore");
        selectedShip.color = ship2;
        resource1.color= ship1;
        resource2.color = MK1Color;
        resource3.color= oreColor;
    }

    public void MK2Ship() {
        scale = 8;
        r1.SetText("2x MK1 Ship");
        r2.SetText("4x MK2 Core");
        r3.SetText("300x Ore");
        selectedShip.color = ship3;
        resource1.color= ship2;
        resource2.color = MK2Color;
        resource3.color= oreColor;
    }

    private void TabReset() {
        colonistTab.SetActive(true);
        missionTab.SetActive(false);
        farmingTab.SetActive(false);
        craftingTab.SetActive(false);
        ColonistTextUpdate();
    }

    public void ColonistTab() {
        colonistTab.SetActive(true);
        missionTab.SetActive(false);
        farmingTab.SetActive(false);
        craftingTab.SetActive(false);
        rename.text = "";
        ColonistTextUpdate();
    }

    public void MissionTab() {
        colonistTab.SetActive(false);
        missionTab.SetActive(true);
        farmingTab.SetActive(false);
        craftingTab.SetActive(false);
    }

    public void FarmingTab() {
        colonistTab.SetActive(false);
        missionTab.SetActive(false);
        farmingTab.SetActive(true);
        craftingTab.SetActive(false);
        FarmingTextUpdate();
    }

    public void CraftingTab() {
        colonistTab.SetActive(false);
        missionTab.SetActive(false);
        farmingTab.SetActive(false);
        craftingTab.SetActive(true);
    }

    public void SaveColonistChanges() {
        GameObject col = new GameObject();
        var name = colonistSelectCT.options[colonistSelectCT.value].text;
        List<String> names = new List<string>();
        foreach (var t in colonistList) {
            names.Add(t.GetComponent<StateManager>().colName);
        }
        for (int i = 0; i < colonistList.Count; i++) {
            if (names[i] == name) {
                col = colonistList[i];
            }
        }
        String newName = rename.text;
        col.GetComponent<StateManager>().colName = newName;
        var colorText = colonistColorSelect.GetComponent<TMP_Dropdown>().options[colonistColorSelect.GetComponent<TMP_Dropdown>().value].text;
        col.GetComponent<StateManager>().SetColor(colorText);
        rename.text = "";
        ColonistUpdate();
        colonistSelectCT.RefreshShownValue();
    }

    private void FarmingTextUpdate() {
        ColonistUpdate();
        int totalBuildings = (topography.GetComponent<TopographyGeneration>().CountBuildings(20));
        int farmingColonists = 0;
        for (int i = 0; i < colonistList.Count; i++) {
            if (colonistList[i].GetComponent<StateManager>().busyState.index == 11 && colonistList[i].GetComponent<StateManager>().currentState == colonistList[i].GetComponent<StateManager>().busyState) {
                farmingColonists += 1;
            }
        }
        openFarms.SetText((totalBuildings - farmingColonists).ToString());
        totalFarms.SetText(totalBuildings.ToString());
    }

    private void MissionTextUpdate() {
        switch (scale) {
            case 1:
                ore.SetText(50+"x");
                crystal.SetText(50+"x");
                food.SetText(50+"x");
                missionShip.color = ship1;
                break;
            case 2:
                ore.SetText(75+"x");
                crystal.SetText(75+"x");
                food.SetText(75+"x");
                missionShip.color = ship1;
                break;
            case 3:
                ore.SetText(125+"x");
                crystal.SetText(125+"x");
                food.SetText(125+"x");
                missionShip.color = ship2;
                break;
            case 4:
                ore.SetText(175+"x");
                crystal.SetText(175+"x");
                food.SetText(175+"x");
                missionShip.color = ship2;
                break;
            case 5:
                ore.SetText(500+"x");
                crystal.SetText(500+"x");
                food.SetText(500+"x");
                missionShip.color = ship3;
                break;
        }
    }

    public void ColonistTextUpdate() {
        colonistSelectCT.RefreshShownValue();
        var name = colonistSelectCT.options[colonistSelectCT.value].text;
        List<String> names = new List<string>();
        foreach (var t in colonistList) {
            names.Add(t.GetComponent<StateManager>().colName);
        }
        for (int i = 0; i < colonistList.Count; i++) {
            if (names[i] == name) {
                col = colonistList[i];
            }
        }
        List<int> traits = new List<int>();
        traits = col.GetComponent<ColonistGridMovement>().traits;
        T1.SetText(""+traits[0]);
        T2.SetText(""+traits[1]);
        T3.SetText(""+traits[2]);
        T4.SetText(""+traits[3]);
        T5.SetText(""+traits[4]);
        Color color = col.GetComponent<StateManager>().colColor;
        colonistImage.color = color;
    }
}