using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
  public InteractTarget target;
  bool TargetSetThisFrame = false;
  public Text interactNameText;
  private void OnTriggerStay(Collider other)
  {
    InteractTarget testTarget = other.GetComponent<InteractTarget>();
    if (testTarget && testTarget.enabled)
    {
      Ray ray = new Ray(transform.position, other.transform.position - transform.position);
      Debug.DrawRay(ray.origin, ray.direction, new Color(0, 0, 0, .25f));
      bool isTrigger = false;
      if (other.isTrigger)
      {
        other.isTrigger = false; //change trigger to not-trigger for raycasting
        isTrigger = true;
      }
      if (Physics.Raycast(ray, out RaycastHit hit, 5.0f, ~0, QueryTriggerInteraction.Ignore))
      {
        Debug.DrawLine(ray.origin, hit.point);
        if (hit.collider == other)
        {
          //TODO: priority system
          target = testTarget;
          TargetSetThisFrame = true;
        }
        else
        {
          //Debug.Log(hit.collider.name);
        }
      }
      if (isTrigger)
      {
        other.isTrigger = true; //...and back to trigger
      }
    }
  }

  private void LateUpdate()
  {
    if (target != null)
    {
      string text = target.HoverName;
      text = text.Replace("\\n", "\n");
      interactNameText.text = text;
    }
    else
    {
      interactNameText.text = "";
    }

    if (!GlobalTools.Paused)
    {
      if (TargetSetThisFrame)
      {
        TargetSetThisFrame = false;
      }
      else
      {
        target = null; //reset at the end of the frame (leave it be while paused)
      }
    }
  }
}
