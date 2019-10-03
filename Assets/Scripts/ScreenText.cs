using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ScreenText : MonoBehaviour
{
    public static ScreenText screenText;

    public Text textOb;
    public float DefaultDisplayTime = 5;

    Canvas canvas;
    static float DisplayTimer;
    static bool Paused = false;
    bool WaitAFrame = false;
    GameObject previewOb;

    private void Awake()
    {
        screenText = this;
        canvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        canvas.enabled = false;
    }

    public static void DisplayText(string text, bool pause = true, float displayTime = -1, GameObject showObject = null)
    {
        screenText.WaitAFrame = true;
        //Debug.Log(text);
        screenText.canvas.enabled = true;
        screenText.textOb.text = text;
        if (showObject)
        {
            screenText.previewOb = GameObject.Instantiate(showObject, screenText.canvas.transform);
            InventoryManager.eventSystem.SetSelectedGameObject(screenText.canvas.gameObject);
        }
        if (pause)
        {
            GlobalTools.Pause();
            Paused = true;
        }
        else
        {
            if (displayTime <= 0) displayTime = screenText.DefaultDisplayTime;
            DisplayTimer = displayTime;
        }

    }

    private void Update()
    {
        if (canvas.enabled && screenText.WaitAFrame)
        {
            WaitAFrame = false;
        }
        else if (canvas.enabled)
        {
            if (Paused)
            {
                if (GlobalTools.inputsGameplay.FindAction("Submit").triggered || GlobalTools.inputsGameplay.FindAction("Cancel").triggered)
                {
                    if (previewOb != null)
                    {
                        Destroy(previewOb);
                    }
                    canvas.enabled = false;
                    GlobalTools.Unpause();
                    Paused = false;
                }
            }
            else
            {
                if (DisplayTimer > 0)
                {
                    DisplayTimer -= Time.deltaTime;
                }
                else
                {
                    if (previewOb!= null)
                    {
                        Destroy(previewOb);
                    }
                    canvas.enabled = false;
                }
            }
        }
    }
}
