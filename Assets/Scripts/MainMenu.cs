using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{
  public Button ContinueButton;
  public MenuManager menuManager;

  public GameObject frontMenu;
  public GameObject bodyType;
  public GameObject skinColor;

  public Color[] SkinColors; //C8AA80 7C5D32 74742D FFFF64
  public GameObject[] bodies;
  public string ShirtColor = "#D9D8E7";
  public string PantsColor = "#7B7BD6";
  private void Awake()
  {
    frontMenu.SetActive(false);
    bodyType.SetActive(false);
    skinColor.SetActive(false);

  }
  public void Start()
  {
    SaveManager.UpdateSavePath();
    Debug.Log(SaveManager.SaveName);
    if (File.Exists(SaveManager.SaveName))
    {
      ContinueButton.gameObject.SetActive(true);
      menuManager.selectOnWake = ContinueButton.gameObject;
    }
    else
    {
      ContinueButton.gameObject.SetActive(false);
    }
  }

  public void NewGameButton()
  {
    SaveManager.ClearSaveData();
    frontMenu.SetActive(false);
    bodyType.SetActive(true);
  }

  public void SelectBodyType(int type)
  {
    SaveManager.BodyType = type;
    bodyType.SetActive(false);
    skinColor.SetActive(true);
    if (type > 0)
    {
      foreach (SkinnedMeshRenderer mesh in skinColor.GetComponentsInChildren<SkinnedMeshRenderer>())
      {
        mesh.SetBlendShapeWeight(type - 1, 100);
      }
    }
    for (int i = 0; i < bodies.Length; i++)
    {
      foreach (SkinnedMeshRenderer mesh in bodies[i].GetComponentsInChildren<SkinnedMeshRenderer>())
      {
        mesh.material.color = SkinColors[i];
        mesh.material.SetColor("_Emission", SkinColors[i]);
      }
    }
  }

  public void SelectSkinColor(int col)
  {
    SaveManager.SkinColor = $"#{ColorUtility.ToHtmlStringRGB(SkinColors[col])}";
    LoadNewGame();
  }

  public void LoadNewGame()
  {
    LevelLoader.ClearTemp = true;
    string[] levelTags = { "newgame" };
    LevelLoader.LoadLevel("Apartment", levelTags);
  }

  public void LoadGame()
  {
    LevelLoader.ClearTemp = true;
    SaveManager.LoadSaveFile();
    LevelLoader.LoadLevel(SaveManager.scene, new string[] { SaveManager.SaveLocation });
  }
  public void LoadScene(string sceneName)
  {
    LevelLoader.LoadLevel(sceneName);
  }
}
