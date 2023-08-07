/* ds18635 2101128
 * ======================
 * This class would pause the game by scaling time down to 0. Further the class would check if there are any colonists
 * busy at the moment of pausing the game, informing the player before they leave the game as live tasks are not able
 * to be saved.
 * ======================
 */
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public GameObject pauseUI;
    public GameObject saveHandler;
    public GameObject playerTaskHandler;
    public TMP_Text busyColonistsInfo;
    private List<String> colonistNames;
    private List<GameObject> colonistList;
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            pauseUI.SetActive(!pauseUI.activeSelf);
            CheckColonists();
        }
        Time.timeScale = pauseUI.activeSelf ? 0 : 1;
    }

    void CheckColonists() {
        colonistList = new List<GameObject>();
        colonistList.AddRange(GameObject.FindGameObjectsWithTag("Colonist"));
        colonistNames = new List<string>();
        for (int i = 0; i < colonistList.Count; i++) {
            if (colonistList[i].GetComponent<StateManager>().currentState == colonistList[i].GetComponent<StateManager>().busyState) {
                colonistNames.Add(colonistList[i].GetComponent<StateManager>().colName);
            }
        }

        if (colonistNames.Count > 0) {
            String busyColonists = colonistNames[0];
            for (int i = 1; i < colonistNames.Count; i++) {
                busyColonists += "& " + colonistNames[i];
            }
            busyColonistsInfo.SetText("Are you sure you want to Quit?\n" + busyColonists + " are still busy & will be interrupted!");
        }
        else {
            busyColonistsInfo.SetText("No busy colonists!");
        }
    }

    public void Resume() {
        pauseUI.SetActive(false);
    }

    public void SaveQuit() {
        playerTaskHandler.GetComponent<PlayerTaskHandler>().dissoResearch.Add(2);
        saveHandler.GetComponent<SaveSystem>().SaveFromPause();
        SceneManager.LoadScene(0);
    }
} 
