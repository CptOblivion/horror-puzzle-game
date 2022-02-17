using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePlayerObjects : MonoBehaviour
{
  //placed on the root canvas for the cutscene overlay (Rendering Packakge -> Interface -> CutsceneOverlay
  public GameObject HUD;
  public GameObject cutscenePause;
  public UnityEngine.UI.Text dialogueText;
  public GameObject DialogueContinueArrow;

  public static CutscenePlayerObjects cutscenePlayerObjects;

  void Awake()
  {
    CutscenePlayerObjects.cutscenePlayerObjects = this;
    //disable the canvas at start
    gameObject.SetActive(false);
  }
}
