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
    public static InventoryItem[] Inventory;
    public int inventoryGridWidth = 3;
    public int inventoryGridHeight = 3;
    public Button defaultButton;
    public Text itemTitle;
    public Text itemDescription;
    public Canvas inventoryScreenCanvas;
    public float ItemSpinSpeed = 180;
    public static List<GameObject> CancelOrder = new List<GameObject>();
    public static GameObject CancelOrderCurrent;
    public static bool CancelTap = false;

    public static InventoryManager inventoryManager;
    public static EventSystem eventSystem;

    GameObject LastSelected;
    bool InventoryOpen = false;
    int[,] slotGrid;
    private void Awake()
    {
        if (Inventory == null)
        {
            Inventory = new InventoryItem[InventoryList.Length];
        }
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
            if(GlobalTools.inputsGameplay.FindAction("Cancel").triggered)
            {
                OpenInventory();
            }
        }
        else
        {
            if (InventoryOpen)
            {
                if (CanCancel(this.gameObject))
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

    private void LateUpdate()
    {
        if (CancelTap)
        {
            CancelTap = false;
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
        CancelOrder = new List<GameObject> { inventoryManager.gameObject };
        CancelOrderCurrent = inventoryManager.gameObject;
    }
    public void CloseInventory()
    {
        GlobalTools.Unpause();
        InventoryOpen = false;
        inventoryScreenCanvas.gameObject.SetActive(false);
    }
    void UpdateInventoryScreen()
    {
        
        for(int i = 0; i <Inventory.Length; i++)
        {
            InventorySlot slot = InventoryList[i];
            slot.item = Inventory[i];
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
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null)
            {
                Inventory[i] = item;
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
        for (; i < Inventory.Length; i++)
        {
            if (Inventory[i].gameObject.name == itemName)
            {
                Inventory[i] = null;
                removed = true;
                break;
            }
        }
        if (removed)
        {
            for (; i < Inventory.Length-1; i++)
            {
                if (Inventory[i + 1] == null)
                {
                    break;
                }
                else
                {
                    Inventory[i] = Inventory[i + 1];
                }
            }
            Inventory[i] = null;
            //inventoryManager.UpdateInventoryScreen();
        }
    }

    public static void SetCancelOwner(GameObject obj)
    {
        CancelOrder.Add(obj);
        CancelOrderCurrent = obj;
    }
    public static void DecrementCancelOrder(GameObject obj = null)
    {
        if (obj != null)
        {
            int index = CancelOrder.IndexOf(obj);
            CancelOrder.RemoveRange(index, CancelOrder.Count - index);
        }
        else
        {
            CancelOrder.RemoveAt(CancelOrder.Count - 1);
        }
        CancelOrderCurrent = CancelOrder[CancelOrder.Count - 1];
        CancelTap = true;
    }
    public static bool CanCancel(GameObject obj)
    {
        if (!CancelTap && CancelOrderCurrent == obj && GlobalTools.inputsGameplay.FindAction("Cancel").triggered)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
