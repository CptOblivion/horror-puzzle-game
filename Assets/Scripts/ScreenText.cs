using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenText : MonoBehaviour
{
    public static ScreenText screenText;

    public Text textOb;
    public float DefaultDisplayTime = 5;

    Canvas canvas;
    static float DisplayTimer;
    static bool Paused = false;
    bool WaitAFrame = false;

    private void Awake()
    {
        screenText = this;
        canvas = GetComponent<Canvas>();
    }
    private void Start()
    {
        canvas.enabled = false;
    }

    public static void DisplayText(string text, bool pause = true, float displayTime = -1)
    {
        screenText.WaitAFrame = true;
        //Debug.Log(text);
        screenText.canvas.enabled = true;
        screenText.textOb.text = text;
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
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.Return))
                {
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
                    canvas.enabled = false;
                }
            }
        }
    }
}
