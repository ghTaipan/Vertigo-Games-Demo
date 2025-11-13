using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    // sprite is not saveable
    public Dictionary<string, int> itemMap = new();
    public int level;
    // this is for cheating by quiting game, only be save ever after player clicks leave button.
    public bool bSafe;

    public SaveData(Dictionary<string, ItemData> itemMap, int level, bool bSafe)
    {
        this.level = level;
        this.bSafe = bSafe;
        foreach (var pair in itemMap)
        {
            this.itemMap.TryAdd(pair.Key, pair.Value._itemCount);
        }
    }
}

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.txt";

    public static void SaveGame(SaveData saveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, saveData);
        stream.Close();
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(path) && new FileInfo(path).Length > 0)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found.");
            return null;
        }
    }
}
