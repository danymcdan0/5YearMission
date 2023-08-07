/* ds18635 2101128
 * ======================
 * This class handles spawning in new colonists and loading in colonists from a save, by first creating them followed by
 * finding a colonist with no information and giving it information provided by the SaveSystem.
 * Further the class sets the values of variables that would otherwise be treated as new, such as the topography,
 * map generation and total resources.
 * ======================
 */
using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadHandler : MonoBehaviour {
    public static string seed;
    public static int[,] map;
    public GameObject myColonist;
    public List<GameObject> colonistList;
    public GameObject playerTaskHandler;
    public void LoadColonists(List<Vector3> pos, int count, List<int> traits, List<string> colors, List<string> names,
        List<int> progression) {
        for (var i = 0; i < count; i++) Instantiate(myColonist, pos[i], Quaternion.identity);
        PlayerTaskHandler.ColonistUpdate();
        var traitCount = 0;
        colonistList = new List<GameObject>();
        colonistList.AddRange(GameObject.FindGameObjectsWithTag("Colonist"));
        for (var i = 0; i < count; i++) {
            var getTraits = new List<int>();
            var getProg = new List<int>();
            for (var j = 0; j < 5; j++) {
                getTraits.Add(traits[traitCount]);
                if (progression != null) getProg.Add(progression[traitCount]);
                traitCount++;
            }
            colonistList[i].GetComponent<ColonistGridMovement>().SetTraits(getTraits);
            if (progression == null)
                colonistList[i].GetComponent<ColonistGridMovement>().NewProgression();
            else
                colonistList[i].GetComponent<ColonistGridMovement>().SetProgression(getProg);
            colonistList[i].GetComponent<StateManager>().SetColor(colors[i]);
            colonistList[i].GetComponent<StateManager>().SetName(names[i]);
        }
    } 


    public void SetSeed(string savedSeed) {
        seed = savedSeed;
    }

    public void SetTopography(List<int> topographyList) {
        map = new int[64, 64];
        var count = 0;
        for (var i = 0; i < 64; i++)
        for (var j = 0; j < 64; j++) {
            map[i, j] = topographyList[count];
            count++;
        }
    }
    
    
    public void SpawnColonists(int count, Vector3 hub) {
        for (var i = 0; i < count; i++) Instantiate(myColonist, hub, Quaternion.identity);
        PlayerTaskHandler.ColonistUpdate();
        colonistList.Clear();
        colonistList.AddRange(GameObject.FindGameObjectsWithTag("Colonist"));
        for (int i = 0; i < colonistList.Count; i++) {
            if (colonistList[i].GetComponent<StateManager>().colName == "") {
                String rename = "NewDupe--Rename";
                Debug.LogWarning(rename);
                colonistList[i].GetComponent<StateManager>().SetName(rename);
                colonistList[i].GetComponent<ColonistGridMovement>().GenTraits();
                colonistList[i].GetComponent<ColonistGridMovement>().NewProgression();
                colonistList[i].GetComponent<StateManager>().SetRandomColor();
            }
        }
    }

    public void SetPlayerData(List<int> resources, List<int> disso) {
        playerTaskHandler.GetComponent<PlayerTaskHandler>().dissoResearch = disso;
        playerTaskHandler.GetComponent<PlayerTaskHandler>().totalResources = resources;
        playerTaskHandler.GetComponent<PlayerTaskHandler>().counterrr();
        
    }
}