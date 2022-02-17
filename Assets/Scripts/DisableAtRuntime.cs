using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAtRuntime : MonoBehaviour
{
  [Tooltip("When true, object isn't disabled if the named bool exists and is true in the save file. When false, object isn't disabled if the named bool doesn't exist, or is false.")]
  public bool OnlyEnableIfSaveBool = true;
  [Tooltip("Name of the bool in the save file to determine disabling. Leave blank to ignore this property")]
  public string SaveBool = "";
  private void Awake()
  {
    if (SaveBool == "")
    {
      gameObject.SetActive(false);

    }
    else
    {
      bool StayAwake = (bool)SaveManager.GetBool(SaveBool, true);
      if (!OnlyEnableIfSaveBool) StayAwake = !StayAwake;

      gameObject.SetActive(StayAwake);
    }
  }
}
