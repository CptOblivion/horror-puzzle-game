using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
  public static bool ClearTemp = false;
  public Color PulseColor1;
  public Color PulseColor2;
  public float PulseTime = 1;
  public bool DefaultToMainMenu = true;
  public float FadeInTime = .5f;
  float FadeInTimer;
  public RawImage sceneFade;
  static Texture2D fadeTex;
  string SceneTemp;
  bool FadeDone = false;

  float PulseTimer = 0;

  Camera cam;
  public static string Scene;
  public static string[] LevelTags = new string[0];

  private void Awake()
  {
    cam = GetComponent<Camera>();
  }
  void Start()
  {
    FadeInTimer = FadeInTime;
    GlobalTools.Unpause();
    fadeTex = (Texture2D)sceneFade.texture;
    if (Scene == null)
    {
      ClearTemp = true;
      Debug.Log("No scene assigned!");
      if (DefaultToMainMenu)
      {
        SceneManager.LoadScene("MainMenu");
      }
    }
    if (ClearTemp)
    {
      Color[] pixGrid = new Color[fadeTex.width * fadeTex.height];
      for (int i = 0; i < pixGrid.Length; i++)
      {
        pixGrid[i] = new Color(0, 0, 0, 1);
      }
      fadeTex.SetPixels(pixGrid);
      fadeTex.Apply();
      ClearTemp = false;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (FadeInTimer > 0)
    {
      FadeInTimer -= Time.deltaTime;
      sceneFade.color = new Color(1, 1, 1, FadeInTimer / FadeInTime);
    }
    else if (!FadeDone)
    {
      FadeDone = true;
      sceneFade.color = new Color(1, 1, 1, 0);
      StartCoroutine(LoadScene());
    }
    PulseTimer += Time.deltaTime;
    cam.backgroundColor = Color.Lerp(PulseColor1, PulseColor2, Mathf.Sin(PulseTimer * 2 * Mathf.PI / PulseTime) * .5f + .5f);
  }

  IEnumerator LoadScene()
  {
    if (Scene != null)
    {
      SceneTemp = Scene;
      Debug.Log("Loading " + SceneTemp);
      Scene = null;
    }
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneTemp);
    asyncLoad.allowSceneActivation = false;
    while (asyncLoad.progress != .9f)
    {
      yield return null;
    }
    SaveForFade();
    asyncLoad.allowSceneActivation = true;
  }
  public static void LoadLevel(string SceneName, string[] Tags = null)
  {
    Tags = Tags ?? new string[0];
    Scene = SceneName;
    SaveManager.scene = SceneName;
    SceneManager.LoadScene("LoadingScreen");
    if (GlobalTools.currentCam != null)
    {
      GlobalTools.currentCam.GetComponent<UpdateBG>().SaveImage();
    }
    LevelTags = Tags;
  }
  void SaveForFade()
  {
    Debug.Log("Loaded!");
    RenderTexture.active = cam.targetTexture;
    fadeTex.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
    fadeTex.Apply();
  }
}
