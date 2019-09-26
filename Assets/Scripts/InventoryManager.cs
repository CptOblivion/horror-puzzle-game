using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public GameObject slotContainer;
    //[HideInInspector]
    public InventoryItem item = null;
    //[HideInInspector]
    public GameObject instantiatedObject = null;

}

[System.Serializable]
public class CustomWrongItemText
{
    public InventoryItem Item;
    public string Message = "";
}
public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] InventoryList = new InventorySlot[9];
    public int inventoryGridWidth = 3;
    public int inventoryGridHeight = 3;
    public int CurrentItemIndex = 0;

    public static InventoryManager inventoryManager;

    Canvas canvas;
    bool InventoryOpen = false;
    int[,] slotGrid;
    private void Awake()
    {
        inventoryManager = this;
        canvas = GetComponent<Canvas>();
        slotGrid = new int[inventoryGridWidth, inventoryGridHeight];
        int i = 0;
        for (int y = 0; y < inventoryGridHeight; y++)
        {
            for (int x = 0; x < inventoryGridWidth; x++)
            {
                slotGrid[x, y] = i;
                i++;
            }
        }
    }
    void Start()
    {
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalTools.Paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenInventory();
            }
        }
        else
        {
            if (InventoryOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseInventory();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    UseItem();
                }
            }
        }
    }

    public void OpenInventory()
    {
        GlobalTools.Pause();
        InventoryOpen = true;
        canvas.enabled = true;
        CurrentItemIndex = 0;
        inventoryManager.UpdateInventoryScreen();
    }
    public void CloseInventory()
    {
        GlobalTools.Unpause();
        InventoryOpen = false;
        canvas.enabled = false;
    }
    void UpdateInventoryScreen()
    {
        foreach (InventorySlot slot in InventoryList)
        {
            if (slot.item == null)
            {
                if (slot.instantiatedObject)
                {
                    GameObject.Destroy(slot.instantiatedObject);
                    slot.instantiatedObject = null;
                }
            }
            else if (!slot.instantiatedObject || slot.instantiatedObject.name != slot.item.gameObject.name)
            {
                if (slot.instantiatedObject)
                {
                    GameObject.Destroy(slot.instantiatedObject);
                }
                slot.instantiatedObject = GameObject.Instantiate(slot.item.gameObject, slot.slotContainer.transform);
                slot.instantiatedObject.transform.localPosition = Vector3.zero;
                slot.instantiatedObject.transform.localRotation = Quaternion.identity;
                slot.instantiatedObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    public void UseItem()
    {
        InventoryItem currentItem = InventoryList[CurrentItemIndex].item;
        if (!currentItem)
        {
            return;
        }
        CloseInventory();
        InteractTarget target = GlobalTools.player.GetComponentInChildren<PlayerInteract>().target;
        if (currentItem.Interact)
        {
            if (target == null)
            {
                ScreenText.DisplayText(currentItem.UseOnNothing);

            }
            else
            {
                if (target.KeyItem == currentItem)
                {
                    target.Interact(true);
                    if (target.ConsumeItem)
                    {
                        RemoveItem(currentItem.name);
                    }
                }
                else
                {
                    string text = target.GetWrongItemText(currentItem);
                    if (text == "")
                    {
                        text = currentItem.UseOnWrongThing;
                    }
                    ScreenText.DisplayText(text);
                }

            }
        }
        else
        {
            //just call a message on the item?
            Debug.Log("no case for item can be used anywhere (yet!)");
        }
    }
    public static bool AddItem(InventoryItem item)
    {
        bool Added = false;
        for (int i = 0; i < inventoryManager.InventoryList.Length; i++)
        {
            if (inventoryManager.InventoryList[i].item == null)
            {
                inventoryManager.InventoryList[i].item = item;
                Added = true;
                //inventoryManager.UpdateInventoryScreen();
                break;
            }
        }
        return Added;
    }
    public static void RemoveItem(string itemName)
    {
        bool removed = false;
        int i = 0;
        for (; i < inventoryManager.InventoryList.Length; i++)
        {
            if (inventoryManager.InventoryList[i].item.gameObject.name == itemName)
            {
                inventoryManager.InventoryList[i].item = null;
                removed = true;
                break;
            }
        }
        if (removed)
        {
            for (; i < inventoryManager.InventoryList.Length-1; i++)
            {
                if (inventoryManager.InventoryList[i + 1].item == null)
                {
                    break;
                }
                else
                {
                    inventoryManager.InventoryList[i].item = inventoryManager.InventoryList[i + 1].item;
                }
            }
            inventoryManager.InventoryList[i].item = null;
            //inventoryManager.UpdateInventoryScreen();
        }
    }
}
