using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePlayerObjects : MonoBehaviour
{
    public GameObject HUD;
    public GameObject cutscenePause;
    public UnityEngine.UI.Text dialogueText;
    public GameObject DialogueContinueArrow;

    public static CutscenePlayerObjects cutscenePlayerObjects;

    void Awake()
    {
        CutscenePlayerObjects.cutscenePlayerObjects = this;
        gameObject.SetActive(false);
    }
}
