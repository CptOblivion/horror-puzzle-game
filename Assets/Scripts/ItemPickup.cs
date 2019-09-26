using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;
    public void Pickup()
    {
        if (InventoryManager.AddItem(item))
        {
            ScreenText.DisplayText(item.ItemName);
            GameObject.Destroy(gameObject);
        }
        else
        {
            ScreenText.DisplayText("I can't carry any more things.");
        }
    }
}
