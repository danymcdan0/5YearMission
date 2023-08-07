/* ds18635 2101128
 * ======================
 * This class would handle all of the UI in the first menu scene of the game. Button actions are also handled here with
 * static information being saved for the game in the second scene to dictate whether it is a new game or not.
 * ======================
 */
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;


public class MainMenu : MonoBehaviour {
    public static bool newGame;
    public static string saveName;
    public static List<int> traits;
    public static List<string> names;
    public static List<string> colors;
    public GameObject Main;
    public GameObject CharcaterMenu;
    public GameObject SelectMenu;
    public TMP_Dropdown dropdown;
    public GameObject colonist1;
    public GameObject colonist2;
    public GameObject colonist3;
    private Random random;
    private int value;

    private void Start() {
        GetFiles();
        random = new Random();
        traits = new List<int>();
        names = new List<string>();
        colors = new List<string>();
    }

    public void StartGame() {
        newGame = true;
        var nameObject = colonist1.gameObject.transform.GetChild(0).gameObject;
        var nameText = nameObject.GetComponent<TMP_InputField>().text;
        var colorObject = colonist1.gameObject.transform.GetChild(1).gameObject;
        var colorText = colorObject.GetComponent<TMP_Dropdown>().options[colorObject.GetComponent<TMP_Dropdown>().value]
            .text;
        names.Add(nameText);
        colors.Add(colorText);
        var nameObject2 = colonist2.gameObject.transform.GetChild(0).gameObject;
        var nameText2 = nameObject2.GetComponent<TMP_InputField>().text;
        var colorObject2 = colonist2.gameObject.transform.GetChild(1).gameObject;
        var colorText2 = colorObject2.GetComponent<TMP_Dropdown>()
            .options[colorObject2.GetComponent<TMP_Dropdown>().value].text;
        names.Add(nameText2);
        colors.Add(colorText2);
        var nameObject3 = colonist3.gameObject.transform.GetChild(0).gameObject;
        var nameText3 = nameObject3.GetComponent<TMP_InputField>().text;
        var colorObject3 = colonist3.gameObject.transform.GetChild(1).gameObject;
        var colorText3 = colorObject3.GetComponent<TMP_Dropdown>()
            .options[colorObject3.GetComponent<TMP_Dropdown>().value].text;
        names.Add(nameText3);
        colors.Add(colorText3);
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ColonistSelection() {
        Main.SetActive(false);
        CharcaterMenu.SetActive(true);
        ReRoll();
    }

    public void LoadMenu() {
        newGame = false;
        Main.SetActive(false);
        SelectMenu.SetActive(true);
    }

    public void LoadGame() {
        saveName = dropdown.options[dropdown.value].text;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Back() {
        Main.SetActive(true);
        SelectMenu.SetActive(false);
        CharcaterMenu.SetActive(false);
    }

    public void ReRoll() {
        traits.Clear();
        names.Clear();
        colors.Clear();
        var miningObject = colonist1.gameObject.transform.GetChild(2).gameObject;
        var craftingObject = colonist1.gameObject.transform.GetChild(3).gameObject;
        var intelligenceObject = colonist1.gameObject.transform.GetChild(4).gameObject;
        var farmingObject = colonist1.gameObject.transform.GetChild(5).gameObject;
        var combatObject = colonist1.gameObject.transform.GetChild(6).gameObject;
        miningObject.GetComponent<TMP_Text>().SetText("Mining: " + RandomValue());
        traits.Add(value);
        craftingObject.GetComponent<TMP_Text>().SetText("Crafting: " + RandomValue());
        traits.Add(value);
        intelligenceObject.GetComponent<TMP_Text>().SetText("Intelligence: " + RandomValue());
        traits.Add(value);
        farmingObject.GetComponent<TMP_Text>().SetText("Farming: " + RandomValue());
        traits.Add(value);
        combatObject.GetComponent<TMP_Text>().SetText("Combat: " + RandomValue());
        traits.Add(value);        
        var miningObject2 = colonist2.gameObject.transform.GetChild(2).gameObject;
        var craftingObject2 = colonist2.gameObject.transform.GetChild(3).gameObject;
        var intelligenceObject2 = colonist2.gameObject.transform.GetChild(4).gameObject;
        var farmingObject2 = colonist2.gameObject.transform.GetChild(5).gameObject;
        var combatObject2 = colonist2.gameObject.transform.GetChild(6).gameObject;
        miningObject2.GetComponent<TMP_Text>().SetText("Mining: " + RandomValue());
        traits.Add(value);
        craftingObject2.GetComponent<TMP_Text>().SetText("Crafting: " + RandomValue());
        traits.Add(value);
        intelligenceObject2.GetComponent<TMP_Text>().SetText("Intelligence: " + RandomValue());
        traits.Add(value);
        farmingObject2.GetComponent<TMP_Text>().SetText("Farming: " + RandomValue());
        traits.Add(value);
        combatObject2.GetComponent<TMP_Text>().SetText("Combat: " + RandomValue());
        traits.Add(value);
        var miningObject3 = colonist3.gameObject.transform.GetChild(2).gameObject;
        var craftingObject3 = colonist3.gameObject.transform.GetChild(3).gameObject;
        var intelligenceObject3 = colonist3.gameObject.transform.GetChild(4).gameObject;
        var farmingObject3 = colonist3.gameObject.transform.GetChild(5).gameObject;
        var combatObject3 = colonist3.gameObject.transform.GetChild(6).gameObject;
        miningObject3.GetComponent<TMP_Text>().SetText("Mining: " + RandomValue());
        traits.Add(value);
        craftingObject3.GetComponent<TMP_Text>().SetText("Crafting: " + RandomValue());
        traits.Add(value);
        intelligenceObject3.GetComponent<TMP_Text>().SetText("Intelligence: " + RandomValue());
        traits.Add(value);
        farmingObject3.GetComponent<TMP_Text>().SetText("Farming: " + RandomValue());
        traits.Add(value);
        combatObject3.GetComponent<TMP_Text>().SetText("Combat: " + RandomValue());
        traits.Add(value);
    }

    private int RandomValue() {
        value = random.Next(9);
        return value;
    }

    private void GetFiles() {
        if (Directory.Exists(FileHandler.SAVE_LOCATION)) {
            dropdown.GetComponent<Dropdown>();
            var directoryInfo = new DirectoryInfo(FileHandler.SAVE_LOCATION);
            var saveFiles = directoryInfo.GetFiles("*.txt").OrderByDescending(p => p.CreationTime).ToArray();
            var fileOptions = new List<string>();
            foreach (var fileInfo in saveFiles) fileOptions.Add(fileInfo.Name);
            dropdown.ClearOptions();
            dropdown.AddOptions(fileOptions);
        }
        else {
            Debug.LogWarning("No saves yet");
        }
    }
} 