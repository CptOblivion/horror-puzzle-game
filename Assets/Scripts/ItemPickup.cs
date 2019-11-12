using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;
    public string SaveName;

    public void Start()
    {
        SaveName = SceneManager.GetActiveScene().name + " PICKUP " + item.name;
        bool? collected = SaveManager.GetBool(SaveName);
        if (collected == true)
        {
            GameObject.Destroy(gameObject);
        }
    }
    public void Pickup()
    {
        if (InventoryManager.AddItem(item))
        {
            ScreenText.DisplayText(item.ItemName, showObject:item.gameObject);
            SaveManager.SetBool(SaveName, true);
            GameObject.Destroy(gameObject);
        }
        else
        {
            ScreenText.DisplayText("I can't carry any more things.");
        }
    }
}
