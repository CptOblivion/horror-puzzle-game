using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelGridControls : MonoBehaviour
{
  public float BarrelAmount = -.05f;
  RawImage rawImage;

  void Start()
  {
    rawImage = GetComponent<RawImage>();
    UpdateBarrel();
  }

  public void ToggleBarrel()
  {
    if (PlayerPrefs.GetInt("Barrel distortion") == 0)
      PlayerPrefs.SetInt("Barrel distortion", 1);
    else
      PlayerPrefs.SetInt("Barrel distortion", 0);
    //GlobalTools.DefaultBarrelDistortion = !GlobalTools.DefaultBarrelDistortion;
    UpdateBarrel();
  }

  void UpdateBarrel()
  {
    rawImage.material.SetFloat("_Barrel", PlayerPrefs.GetInt("Barrel distortion") * BarrelAmount);

  }
}
