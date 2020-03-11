using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int SaveVersionNumber = 2;

    public Dictionary<string, bool> SaveFlagsBool = new Dictionary<string, bool>();
    public Dictionary<string, int> SaveFlagsInt = new Dictionary<string, int>();
    public Dictionary<string, float> SaveFlagsFloat = new Dictionary<string, float>();
    public Dictionary<string, string> SaveFlagsString = new Dictionary<string, string>();
    public List<string> Inventory;
    public string Scene;
    public string SaveLocation; //think of a better name! This refers to the name of the location in the level where the save was made (EG the chair at the apartment)
    public string SkinColor;
    public string ShirtColor;
    public string PantsColor;
    public int BodyType;

    // TO ADD NEW VARIABLES, USE THE FOLLOWING (increment the version number, and match above):
    //[System.Runtime.Serialization.OptionalField(VersionAdded = 2)]
}

public class SaveDataUpdateHelper
{
    public enum Types {NOTYPE, boolType, intType, floatType, stringType};
    public AccessesSaveData obj = null;
    public string Property = null;
    public Types type = Types.NOTYPE;
}

public interface AccessesSaveData
{
    void SavePropertyUpdated();
}
public class SaveManager:MonoBehaviour
{
    static SaveData saveData = new SaveData();
    public static int SaveVersionNumber;
    public static string scene;
    public static string SaveLocation;
    public static string SkinColor;
    public static string ShirtColor;
    public static string PantsColor;
    public static int BodyType;
    public static float? LastSaveTime;
    public static int SaveSlot = 0;
    public static string SaveName;
    public static SaveManager saveManager;
    List<SaveDataUpdateHelper> ObjectsToUpdate;

    private void Awake()
    {
        saveManager = this;
        UpdateSavePath();
        ObjectsToUpdate = new List<SaveDataUpdateHelper>();
    }

    public static void UpdateSavePath()
    {
         SaveName = $"{Application.persistentDataPath}/SaveGame{SaveSlot}.sav";
    }

    public static void LoadSaveFile()
    {
        if (File.Exists($"{Application.persistentDataPath}/SaveGame{SaveSlot}.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/SaveGame{SaveSlot}.sav", FileMode.Open);

            saveData = (SaveData)bf.Deserialize(file);
            file.Close();


            SaveVersionNumber = saveData.SaveVersionNumber;
            Debug.Log($"Save file version number {SaveVersionNumber}");

            scene = saveData.Scene;
            SaveLocation = saveData.SaveLocation;
            SkinColor = saveData.SkinColor;
            ShirtColor = saveData.ShirtColor;
            PantsColor = saveData.PantsColor;
            BodyType = saveData.BodyType;

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

    public static void SaveSaveFile()
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
        saveData.SkinColor = SkinColor;
        saveData.ShirtColor = ShirtColor;
        saveData.PantsColor = PantsColor;
        saveData.BodyType = BodyType;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/SaveGame{SaveSlot}.sav");
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
        //caucasian: #C8AA80
        SkinColor = "#C8AA80";
        //white shirt: #D9D8E7
        ShirtColor = "#D9D8E7";
        //jeans: #7B7BD6
        PantsColor = "#7B7BD6";
        BodyType = 1;
    }

    public static void AddObjectToUpdate(AccessesSaveData obj, string PropName, SaveDataUpdateHelper.Types type)
    {
        if (saveManager != null)
        {
            SaveDataUpdateHelper newHelper = new SaveDataUpdateHelper();
            newHelper.obj = obj;
            newHelper.Property = PropName;
            newHelper.type = type;
            saveManager.ObjectsToUpdate.Add(newHelper);
        }
    }

    void UpdateObjects(string PropName, SaveDataUpdateHelper.Types type)
    {
        for (int i = 0; i < ObjectsToUpdate.Count; i++)
        {
            if (ObjectsToUpdate[i].Property == PropName && type == ObjectsToUpdate[i].type)
            {
                ObjectsToUpdate[i].obj.SavePropertyUpdated();
            }
        }
    }

    public static bool? GetBool(string boolName, bool FalseIfNull = false)
    {
        if (saveData.SaveFlagsBool.ContainsKey(boolName))
            return saveData.SaveFlagsBool[boolName];
        else
            if (FalseIfNull) return false;
            else return null;
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
        if (saveManager)
        {
            saveManager.UpdateObjects(boolName, SaveDataUpdateHelper.Types.boolType);
        }
    }
    public static void SetInt(string intName, int value)
    {
        saveData.SaveFlagsInt[intName] = value;
        if (saveManager)
        {
            saveManager.UpdateObjects(intName, SaveDataUpdateHelper.Types.intType);
        }
    }
    public static void SetFloat(string floatName, float value)
    {
        saveData.SaveFlagsFloat[floatName] = value;
        if (saveManager)
        {
            saveManager.UpdateObjects(floatName, SaveDataUpdateHelper.Types.floatType);
        }
    }
    public static void SetString(string stringName, string value)
    {
        saveData.SaveFlagsString[stringName] = value;
        if (saveManager)
        {
            saveManager.UpdateObjects(stringName, SaveDataUpdateHelper.Types.stringType);
        }
    }
}
