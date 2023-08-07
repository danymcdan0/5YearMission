/* ds18635 2101128
 * ======================
 * This class handles the pre-initialisation of the second scene (Awake method) by checking the static values in the
 * MainMenu avoiding any Null errors with classes that rely on others. With the main value being a newGame Boolean,
 * dictating the order of classes to be loaded or initialised (new game or continuation).
 * ======================
 */
using System.Collections.Generic;
using UnityEngine;

public class MainHandler : MonoBehaviour {
    private static bool newGame;
    public GameObject mapGeneration;
    public GameObject topographyGeneration;
    public GameObject saveHandler;
    public GameObject loadHandler;
    public GameObject playerTaskHandler;

    private void Awake() {
        newGame = MainMenu.newGame;
        saveHandler.GetComponent<SaveSystem>().LoadData();
        if (newGame) {
            Debug.LogWarning("New Game");
            mapGeneration.GetComponent<MapGeneration>().useRandomSeed = true;
            playerTaskHandler.GetComponent<PlayerTaskHandler>().totalResources = new List<int>();
            for (int i = 0; i < 3; i++) {
                playerTaskHandler.GetComponent<PlayerTaskHandler>().totalResources.Add(300);
            }
            for (int i = 0; i < 7; i++) {
                playerTaskHandler.GetComponent<PlayerTaskHandler>().totalResources.Add(0);
            }
        }
        else {
            Debug.LogWarning("Old Game");
            mapGeneration.GetComponent<MapGeneration>().useRandomSeed = false;
            mapGeneration.GetComponent<MapGeneration>().setSeed(LoadHandler.seed);
        }
        mapGeneration.GetComponent<MapGeneration>().GenerateMap();
    } 

    private void Start() {
        topographyGeneration.GetComponent<TopographyGeneration>().width =
            mapGeneration.GetComponent<MapGeneration>().width;
        topographyGeneration.GetComponent<TopographyGeneration>().height =
            mapGeneration.GetComponent<MapGeneration>().height;
        topographyGeneration.GetComponent<TopographyGeneration>().Init();
        if (newGame) {
            topographyGeneration.GetComponent<TopographyGeneration>().topography =
                mapGeneration.GetComponent<MapGeneration>().map;
            topographyGeneration.GetComponent<TopographyGeneration>().SpawnClearing();
            topographyGeneration.GetComponent<TopographyGeneration>().seed =
                mapGeneration.GetComponent<MapGeneration>().seed;
            var pos = new List<Vector3>();
            pos = topographyGeneration.GetComponent<TopographyGeneration>().spawnPoints();
            loadHandler.GetComponent<LoadHandler>()
                .LoadColonists(pos, 3, MainMenu.traits, MainMenu.colors, MainMenu.names, null);
        }
        else {
            topographyGeneration.GetComponent<TopographyGeneration>().setMap(LoadHandler.map);
            topographyGeneration.GetComponent<TopographyGeneration>().seed = LoadHandler.seed;
            saveHandler.GetComponent<SaveSystem>().Load(); //TODO
        }
        mapGeneration.GetComponent<MapGeneration>().GenerateMap();
    }

    private void Update() {
        topographyGeneration.GetComponent<TopographyGeneration>().Generate();
    }
}