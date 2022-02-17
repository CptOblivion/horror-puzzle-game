using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RevealSubmenu : MonoBehaviour
{
  public Transform submenuParent;
  public Button autoSelect;

  private void Awake()
  {
    this.enabled = false;
    submenuParent.gameObject.SetActive(false);
  }
  private void OnEnable()
  {
    submenuParent.gameObject.SetActive(true);
    InventoryManager.eventSystem.SetSelectedGameObject(autoSelect.gameObject);
    InventoryManager.SetCancelOwner(gameObject);
    //Debug.Log(InventoryManager.CancelOrder);
  }
  void Update()
  {
    bool selected = false;
    foreach (Transform child in submenuParent.GetComponentsInChildren<Transform>())
    {
      if (InventoryManager.eventSystem.currentSelectedGameObject == child.gameObject)
      {
        selected = true;
        break;
      }
    }
    if (InventoryManager.CanCancel(this.gameObject))
    {
      selected = false;
    }
    if (!selected)
    {
      Cancel();
    }
  }

  public void Cancel()
  {
    //Debug.Log("canceling " + gameObject);
    submenuParent.gameObject.SetActive(false);
    InventoryManager.DecrementCancelOrder(gameObject);
    InventoryManager.eventSystem.SetSelectedGameObject(gameObject);
    this.enabled = false;
  }
}
