
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTarget : MonoBehaviour
{
    [Tooltip("Object to call a function on (if blank, calls on self)")]
    public GameObject InteractObject;
    [Tooltip("Function to call")]
    public string Function = "Interact";
    [Tooltip("Text to display when interacting (if no key item assigned)")]
    public string InteractMessage = "";
    [Tooltip("name of the inventory item required to interact with this (leave empty if none)")]
    public InventoryItem KeyItem;
    [Tooltip("What to say when attempting to interact without an item (if a keyitem is assigned)")]
    public string NoItemText = "Locked.";
    [Tooltip("What to say when attempting to interact with the wrong item (leave blank to default to the item's own text)")]
    public string GenericWrongItemText = "";
    [Tooltip("custom overrides for specific items (EG if the item is a glass of water and the target is a houseplant, you might assign the message \"It doesn't look thirsty.\"")]
    public CustomWrongItemText[] customWrongItemText = new CustomWrongItemText[] { };
    [Tooltip("If true, the inventory item is removed upon use")]
    public bool ConsumeItem = true;

    //oneuse targets disable themselves after interacting
    public bool OneUse = false;
    public void Interact(bool useItem = false)
    {
        if (useItem || (!useItem && !KeyItem))
        {
            if (InteractObject)
            {
                InteractObject.SendMessage(Function);
            }
            else
            {
                SendMessage(Function);
            }
            if (OneUse)
            {
                gameObject.SetActive(false);
            }
            if (InteractMessage != "")
            {
                ScreenText.DisplayText(InteractMessage);
            }
        }
        else
        {
            ScreenText.DisplayText(NoItemText);
            Debug.Log(NoItemText);
        }
    }
    public string GetWrongItemText(InventoryItem item)
    {
        string feedback = GenericWrongItemText;
        foreach (CustomWrongItemText wrongItem in customWrongItemText)
        {
            if (wrongItem.Item == item)
            {
                feedback = wrongItem.Message;
                break;
            }
        }
        return feedback;
    }
}
