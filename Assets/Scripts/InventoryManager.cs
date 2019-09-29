using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    [Tooltip("the inventory slot background object")]
    public GameObject slotContainer;
    [HideInInspector]
    public InventoryItem item = null;
    [HideInInspector]
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
    public Button defaultButton;
    public Text itemTitle;
    public Text itemDescription;
    public Canvas inventoryScreenCanvas;
    public float ItemSpinSpeed = 180;

    public static InventoryManager inventoryManager;
    public static EventSystem eventSystem;

    GameObject LastSelected;
    bool InventoryOpen = false;
    int[,] slotGrid;
    private void Awake()
    {
        inventoryManager = this;
        eventSystem = FindObjectOfType<EventSystem>();
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
        inventoryScreenCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalTools.Paused)
        {
            if(GlobalTools.inputsGameplay.GetAction("Pause").triggered)
            {
                OpenInventory();
            }
        }
        else
        {
            if (InventoryOpen)
            {
                if(GlobalTools.inputsMenus.GetAction("Cancel").triggered)
                {
                    CloseInventory();
                }
                if (eventSystem.currentSelectedGameObject != LastSelected)
                {
                    UpdateItemText();
                }
            }
        }
    }

    void UpdateItemText()
    {
        LastSelected = eventSystem.currentSelectedGameObject;
        bool slotSelected = false;
        foreach (InventorySlot slot in InventoryList)
        {
            if (slot.slotContainer == LastSelected)
            {
                if (slot.item)
                {
                    itemTitle.text = slot.item.ItemName;
                    itemDescription.text = slot.item.ItemDescription;
                    slotSelected = true;
                }
                break;
            }
        }
        if (!slotSelected)
        {
            itemTitle.text = "";
            itemDescription.text = "";
        }
    }
    public void OpenInventory()
    {
        GlobalTools.Pause();
        InventoryOpen = true;
        inventoryScreenCanvas.gameObject.SetActive(true);
        inventoryManager.UpdateInventoryScreen();
        if (defaultButton)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(defaultButton.gameObject);
            LastSelected = defaultButton.gameObject;
        }
        UpdateItemText();
    }
    public void CloseInventory()
    {
        GlobalTools.Unpause();
        InventoryOpen = false;
        inventoryScreenCanvas.gameObject.SetActive(false);
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
    public void UseItem(int slot)
    {
        //InventoryItem currentItem = InventoryList[CurrentItemIndex].item;
        InventoryItem currentItem = InventoryList[slot].item;
        //Debug.Log("using item");
        if (!currentItem)
        {
            //Debug.Log("empty slot");
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
