using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveTimeWarning : MonoBehaviour
{
  Text text;

  public string NoSaveMessage = "YOU HAVE NOT SAVED!";
  private void Awake()
  {
    text = GetComponent<Text>();
  }
  private void OnEnable()
  {

    if (SaveManager.LastSaveTime == null)
    {
      text.text = NoSaveMessage;
    }
    else
    {
      int MinutesSinceLastSave = (int)((Time.unscaledTime - SaveManager.LastSaveTime) / 60);
      text.text = "IT HAS BEEN " + MinutesSinceLastSave + " MINUTES\nSINCE YOUR LAST SAVE.";
    }
  }
}
