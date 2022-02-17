using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpinner : MonoBehaviour
{
  public float TipAngle = -45;
  Transform spinOb;
  private void Awake()
  {
    spinOb = GetComponentInChildren<MeshFilter>().transform;
    if (!spinOb)
    {
      Debug.LogError("No mesh found!", this);
      this.enabled = false;
    }
    else
    {
      spinOb = spinOb.parent;
    }
  }

  private void OnEnable()
  {
    spinOb.localEulerAngles = new Vector3(TipAngle, 0, 0);
  }
  void Update()
  {
    if (InventoryManager.eventSystem.currentSelectedGameObject == transform.parent.gameObject)
    {
      spinOb.Rotate(new Vector3(0, Time.unscaledDeltaTime * InventoryManager.inventoryManager.ItemSpinSpeed, 0), Space.Self);
    }
    else
    {
      spinOb.localEulerAngles = new Vector3(TipAngle, 0, 0);
    }
  }
}
