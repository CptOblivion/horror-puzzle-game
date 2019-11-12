using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class SaveData
{
    public Dictionary<string, bool> SaveFlagsBool = new Dictionary<string, bool>();
    public Dictionary<string, int> SaveFlagsInt = new Dictionary<string, int>();
    public Dictionary<string, float> SaveFlagsFloat = new Dictionary<string, float>();
    public Dictionary<string, string> SaveFlagsString = new Dictionary<string, string>();
    public List<string> Inventory;
    public string Scene;
    public string SaveLocation; //think of a better name! This refers to the name of the location in the level where the save was made (EG the chair at the apartment)
}

public class SaveManager
{
    static SaveData saveData = new SaveData();
    public static string scene;
    public static string SaveLocation;
    public static float? LastSaveTime;

    public static void LoadSaveFile(int SaveSlot = 0)
    {
        if (File.Exists(Application.persistentDataPath + "/SaveGame" + SaveSlot))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveGame" + SaveSlot, FileMode.Open);
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();
            scene = saveData.Scene;
            SaveLocation = saveData.SaveLocation;
            if (saveData.Inventory != null)
            {
                InventoryManager.Inventory = new InventoryItem[InventoryManager.InventorySize];
                for(int i = 0; i < saveData.Inventory.Count; i++)
                {
                    GameObject ob = (GameObject)Resources.Load("InventoryItems/" + saveData.Inventory[i]);
                    InventoryManager.Inventory[i] = ob.GetComponent<InventoryItem>();
                }
            }
            else
                InventoryManager.Inventory = null;


        }
    }

    public static void SaveSaveFile(int SaveSlot = 0)
    {
        if (InventoryManager.Inventory != null)
        {
            saveData.Inventory = new List<string>();
            foreach (InventoryItem item in InventoryManager.Inventory)
            {
                if (item != null)
                {
                    Debug.Log(item.gameObject);
                    saveData.Inventory.Add(item.gameObject.name);
                    Debug.Log("item added");
                }

            }
        }
        else
            saveData.Inventory = null;
        saveData.Scene = scene;
        saveData.SaveLocation = SaveLocation;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/SaveGame" + SaveSlot);
        bf.Serialize(file, saveData);
        file.Close();

        LastSaveTime = Time.unscaledTime;
    }

    public static void ClearSaveData()
    {
        saveData = new SaveData();
        InventoryManager.Inventory = null;
        SaveLocation = null;
        scene = null;
    }

    public static bool? GetBool(string boolName)
    {
        if (saveData.SaveFlagsBool.ContainsKey(boolName))
            return saveData.SaveFlagsBool[boolName];
        else
            return null;
    }
    public static int? GetInt(string intName)
    {
        if (saveData.SaveFlagsInt.ContainsKey(intName))
            return saveData.SaveFlagsInt[intName];
        else
            return null;
    }
    public static float? GetFloat(string floatName)
    {
        if (saveData.SaveFlagsFloat.ContainsKey(floatName))
            return saveData.SaveFlagsFloat[floatName];
        else
            return null;
    }
    public static string GetString(string stringName)
    {
        if (saveData.SaveFlagsString.ContainsKey(stringName))
            return saveData.SaveFlagsString[stringName];
        else
            return null;
    }
    public static void SetBool(string boolName, bool value)
    {
        saveData.SaveFlagsBool[boolName] = value;
        Debug.Log("setting bool " + boolName + " to " + value);
    }
    public static void SetInt(string intName, int value)
    {
        saveData.SaveFlagsInt[intName] = value;
    }
    public static void SetFloat(string floatName, float value)
    {
        saveData.SaveFlagsFloat[floatName] = value;
    }
    public static void SetString(string stringName, string value)
    {
        saveData.SaveFlagsString[stringName] = value;
    }
}
