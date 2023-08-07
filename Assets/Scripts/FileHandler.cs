/* ds18635 2101128
 * ======================
 * This class handles creating the save location, then getting the list of files in the Save folder, followed by loading
 * the correct save according to its name. 
 * ======================
 */
using System.IO;
using UnityEngine;

public static class FileHandler {
    public static readonly string SAVE_LOCATION = Application.dataPath + "/Saves/";

    public static void init() {
        if (!Directory.Exists(SAVE_LOCATION)) Directory.CreateDirectory(SAVE_LOCATION);
    } 

    public static void Save(string saveString) {
        var saveCount = 1;
        while (File.Exists(SAVE_LOCATION + "save_" + saveCount + ".txt")) saveCount++;
        Debug.LogWarning("Saved");
        File.WriteAllText(SAVE_LOCATION + "save_" + saveCount + ".txt", saveString);
    }

    public static string Load() {
        var directoryInfo = new DirectoryInfo(SAVE_LOCATION);
        var saveFiles = directoryInfo.GetFiles("*.txt");
        FileInfo mostRecent = null;
        foreach (var fileInfo in saveFiles)
            if (mostRecent == null)
                mostRecent = fileInfo;
            else if (fileInfo.LastWriteTime > mostRecent.LastWriteTime) mostRecent = fileInfo;
        if (mostRecent != null) {
            var saveString = File.ReadAllText(mostRecent.FullName);
            Debug.LogWarning("Loaded");
            return saveString;
        }

        Debug.LogWarning("No Saves");
        return null;
    }

    public static string LoadStart(string saveName) {
        var directoryInfo = new DirectoryInfo(SAVE_LOCATION);
        var saveFiles = directoryInfo.GetFiles("*.txt");
        FileInfo file = null;
        foreach (var fileInfo in saveFiles)
            if (fileInfo.Name == saveName)
                file = fileInfo;

        if (file != null) {
            var saveString = File.ReadAllText(file.FullName);
            Debug.LogWarning("Loaded");
            return saveString;
        }

        Debug.LogWarning("No Saves");
        return null;
    }

}