
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTarget : MonoBehaviour, AccessesSaveData
{
  [Tooltip("Name to show when aiming at the object")]
  public string HoverName = "";
  [Tooltip("Text to display when interacting (if no key item assigned)")]
  public string InteractMessage = "";
  [Tooltip("manually shift the contents of the message vertically")]
  public float MessageOffset = 0;
  [Tooltip("if true, displays a confirmation dialog allowing the player to cancel the interaction")]
  public bool InteractMessageConfirmation = false;
  [Tooltip("Use this camera while displaying the message")]
  public MoveCameraToPosition cameraOverride;
  [Tooltip("To prevent the player character from blocking the camera during closeups")]
  public bool HidePlayer = false;

  [Tooltip("Object to call a function on (if blank, calls on self)")]
  public GameObject InteractObject;
  [Tooltip("Function to call")]
  public string Function = "Interact";

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

  [Tooltip("oneuse targets disable themselves after interacting. The string is the name of the property to use in the save file (be verbose! A long name is better than overlapping properties)")]
  public string OneUse = "";
  [Tooltip("only activate if the named property exists in the save system as a bool and is equal to true (EG pairing with OneUse on another trigger)")]
  public string ActivateOnSaveProperty = "";

  //prevents the function from being called until the displayed text is dismissed
  bool TextDelay = false;

  private void Start()
  {
    if (SaveManager.GetBool(OneUse) == true)
    {
      gameObject.SetActive(false);
    }
    else if (ActivateOnSaveProperty != "")
    {
      SavePropertyUpdated();
      SaveManager.AddObjectToUpdate(this, ActivateOnSaveProperty, SaveDataUpdateHelper.Types.boolType);
    }
  }
  private void Update()
  {
    if (TextDelay && Time.timeScale > 0)
    {
      if (cameraOverride)
      {
        GlobalTools.currentCam.GetComponent<UpdateBG>().UpdateCamera();
        if (HidePlayer)
        {
          GlobalTools.player.GetComponentInChildren<Renderer>().gameObject.layer = (int)GlobalTools.LayerMasks.RenderOBJ;
        }
      }
      if (InteractMessageConfirmation)
      {
        if (ScreenText.SelectedOption == "Confirm")
        {
          Interact(false);
        }
        else
        {
          TextDelay = false;
        }
      }
      else
      {
        Interact(false);
      }
    }
  }

  public void Interact(bool useItem = false)
  {
    if (useItem || (!useItem && !KeyItem))
    {
      if (InteractMessage != "" && TextDelay == false)
      {
        string[] MessageOptions = null;
        if (InteractMessageConfirmation)
        {
          MessageOptions = new string[] { "Cancel", "Confirm" };
        }
        ScreenText.DisplayText(InteractMessage, ButtonOptions: MessageOptions, offset: MessageOffset);
        TextDelay = true;
        if (cameraOverride)
        {
          GlobalTools.currentCam.GetComponent<UpdateBG>().UpdateCamera(cameraOverride.GetComponentInChildren<CameraPosition>());
          if (HidePlayer)
          {
            GlobalTools.player.GetComponentInChildren<Renderer>().gameObject.layer = (int)GlobalTools.LayerMasks.Default;
          }
        }
      }
      else if (InteractObject)
      {
        if (Function != "")
        {
          InteractObject.SendMessage(Function);
        }
        TextDelay = false;
      }
      else
      {
        if (Function != "")
        {
          SendMessage(Function);
        }
        TextDelay = false;
      }

      if (OneUse != "")
      {
        gameObject.SetActive(false);
        SaveManager.SetBool(OneUse, true);
      }
    }
    else
    {
      ScreenText.DisplayText(NoItemText);
      //Debug.Log(NoItemText);
    }
  }
  public string GetWrongItemText(string itemName)
  {
    string feedback = GenericWrongItemText;
    foreach (CustomWrongItemText wrongItem in customWrongItemText)
    {
      if (wrongItem.ItemName == itemName)
      {
        feedback = wrongItem.Message;
        break;
      }
    }
    return feedback;
  }
  public void SavePropertyUpdated()
  {
    if (SaveManager.GetBool(ActivateOnSaveProperty) == true)
    {
      gameObject.SetActive(true);
    }
    else
    {
      gameObject.SetActive(false);
    }
  }
}
