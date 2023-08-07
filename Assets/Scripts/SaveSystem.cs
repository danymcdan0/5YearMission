/* ds18635 2101128
 * ======================
 * This class handles creating the SaveObject to be able to save the important information that the player needs between
 * saves. Further the class gets all the required information to set to the variables in the SaveObject, able to unpack
 * the SaveObject with help from the LoadHandler when loading into a game with a save.
 * ======================
 */
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour {
    public GameObject loadHandler;
    public GameObject topography;
    public GameObject map;
    public GameObject playerTaskHandler;
    private List<GameObject> colonistList;

    private void Awake() {
        colonistList = new List<GameObject>();
        ColonistUpdate();
        FileHandler.init();
    }

    private void ColonistUpdate() {
        colonistList.Clear();
        colonistList.AddRange(GameObject.FindGameObjectsWithTag("Colonist"));
    }

    public void SaveFromPause() {
        Save();
    }

    private void Save() {
        ColonistUpdate();
        var colonistPos = new List<Vector3>();
        var traits = new List<int>();
        var progression = new List<int>();
        var colors = new List<string>();
        var names = new List<string>();
        var totalResources = new List<int>();
        var dissoResearch = new List<int>();

        for (var i = 0; i < colonistList.Count; i++) {
            colonistPos.Add(colonistList[i].transform.position);
            traits.AddRange(colonistList[i].GetComponent<ColonistGridMovement>().traits);
            progression.AddRange(colonistList[i].GetComponent<ColonistGridMovement>().progression);
            if (colonistList[i].GetComponent<StateManager>().colColor == Color.yellow) colors.Add("Yellow");
            if (colonistList[i].GetComponent<StateManager>().colColor == Color.magenta) colors.Add("Magenta");
            if (colonistList[i].GetComponent<StateManager>().colColor == Color.red) colors.Add("Red");
            if (colonistList[i].GetComponent<StateManager>().colColor == Color.green) colors.Add("Green");
            names.Add(colonistList[i].GetComponent<StateManager>().colName);
        } 
        var seed = map.GetComponent<MapGeneration>().seed;
        var top = topography.GetComponent<TopographyGeneration>().gridToInt();
        totalResources = playerTaskHandler.GetComponent<PlayerTaskHandler>().totalResources;
        dissoResearch = playerTaskHandler.GetComponent<PlayerTaskHandler>().dissoResearch;

        var saveObject = new SaveObject {
            traits = traits,
            colonistPostion = colonistPos,
            seed = seed,
            colors = colors,
            names = names,
            topography = top,
            progression = progression,
            resources = totalResources,
            dissoResearch = dissoResearch
        };
        var json = JsonUtility.ToJson(saveObject);
        FileHandler.Save(json);
    }

    public void Load() {
        var saveString = FileHandler.LoadStart(MainMenu.saveName);
        if (saveString != null) {
            var saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            loadHandler.GetComponent<LoadHandler>().SetSeed(saveObject.seed);
            loadHandler.GetComponent<LoadHandler>().SetTopography(saveObject.topography);
            loadHandler.GetComponent<LoadHandler>().LoadColonists(saveObject.colonistPostion,
                saveObject.colonistPostion.Count, saveObject.traits, saveObject.colors, saveObject.names,
                saveObject.progression);
            loadHandler.GetComponent<LoadHandler>().SetPlayerData(saveObject.resources, saveObject.dissoResearch);
        }
        else {
            Debug.Log("No Save");
        }
    }

    public void LoadData() {
        var saveString = FileHandler.LoadStart(MainMenu.saveName);
        if (saveString != null) {
            var saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            Debug.Log(saveObject.seed);
            loadHandler.GetComponent<LoadHandler>().SetSeed(saveObject.seed);
            loadHandler.GetComponent<LoadHandler>().SetTopography(saveObject.topography);
        }
    }

    private class SaveObject {
        public List<Vector3> colonistPostion;
        public List<string> colors;
        public List<string> names;
        public List<int> progression;
        public string seed;
        public List<int> topography;
        public List<int> traits;
        public List<int> dissoResearch;
        public List<int> resources;
    }

    //Keep track of when players reload as they may have lost a colonist and re-affrims attachment
}