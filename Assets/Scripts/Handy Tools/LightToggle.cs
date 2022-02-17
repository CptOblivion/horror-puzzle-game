using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightToggle : MonoBehaviour
{
  Light attachedLight;
  public bool SwitchedOn = true;

  private void Awake()
  {
    attachedLight = GetComponentInChildren<Light>();
  }
  void Start()
  {
    UpdateLights();
  }

  private void OnValidate()
  {
    UpdateLights();
  }

  void UpdateLights()
  {
    if (attachedLight) attachedLight.gameObject.SetActive(SwitchedOn);
  }

  public void ToggleLights()
  {
    SwitchedOn = !SwitchedOn;
  }
}
