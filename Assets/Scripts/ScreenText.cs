using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ScreenText : MonoBehaviour
{
    public static ScreenText screenText;

    public Button buttonTemplate;
    public Text textOb;
    public float DefaultDisplayTime = 5;
    public float Margins = 20;
    public float ButtonGap = 20;

    Canvas canvas;
    static float DisplayTimer;
    static bool Paused = false;
    bool WaitAFrame = false;
    GameObject previewOb;
    public static string SelectedOption = null;

    GameObject HUD;
    RectTransform textParent;

    GameObject buttonParent; //track the button parent object, if it exists

    private void Awake()
    {
        screenText = this;
        canvas = GetComponent<Canvas>();
        textParent = (RectTransform)textOb.transform.parent;
    }
    private void Start()
    {
        canvas.enabled = false;
        HUD = GameObject.Find("HUD");
    }

    public static void DisplayText(string text, bool pause = true, float displayTime = -1, GameObject showObject = null, string[] ButtonOptions = null, float offset = 0, float MarginsOverride = -1)
    {
        //start a list of all the things we're showing on the screen to keep track of what goes where
        List<GameObject> DisplayList = new List<GameObject>();
        screenText.WaitAFrame = true;
        //Debug.Log(text);
        screenText.canvas.enabled = true;
        screenText.HUD.SetActive(false);


        text = text.Replace("\\n", "\n");
        screenText.textOb.text = text;
        Canvas.ForceUpdateCanvases();
        screenText.textParent.sizeDelta = screenText.textOb.rectTransform.sizeDelta + new Vector2(100,100);// = (new Vector2(screenText.textOb.preferredWidth + 120, screenText.textOb.preferredHeight + 20));
        if (showObject)
        {
            screenText.previewOb = GameObject.Instantiate(showObject, screenText.canvas.transform);
            DisplayList.Add(screenText.previewOb); //the preview object goes on top
            InventoryManager.eventSystem.SetSelectedGameObject(screenText.canvas.gameObject);
        }
        DisplayList.Add(screenText.textParent.gameObject); //next up is the text itself (well, the frame it sits in)
        if (pause)
        {
            GlobalTools.Pause();
            Paused = true;
            if (ButtonOptions == null)
            {
                screenText.buttonParent = null;
            }
            else
            {
                //the buttons are arranged under a parent object so they can be close together regardless of how many things are on screen
                screenText.buttonParent = new GameObject("ButtonParent");
                screenText.buttonParent.transform.SetParent(screenText.transform, false);
                DisplayList.Add(screenText.buttonParent); //the buttons go at the bottom

                for (int i = 0; i < ButtonOptions.Length; i++)
                {
                    GameObject ButtonOb = Instantiate(screenText.buttonTemplate.gameObject, screenText.buttonParent.transform);
                    if (i == 0)
                    {
                        EventSystem.current.SetSelectedGameObject(ButtonOb);

                    }
                    ButtonOb.name = ButtonOptions[i];
                    ButtonOb.transform.localPosition = new Vector3(0, i * -screenText.ButtonGap + (ButtonOptions.Length - 1) * screenText.ButtonGap/2 );
                    ButtonOb.GetComponentInChildren<Text>().text = ButtonOptions[i];
                }
            }

            float CanvasHeight = screenText.canvas.rootCanvas.pixelRect.height;
            float margins = screenText.Margins;
            if (MarginsOverride >= 0)
            {
                margins = MarginsOverride;
            }
            float TopMargin = (margins - Mathf.Min(offset, 0)) * CanvasHeight;
            float BottomMargin = (margins + Mathf.Max(offset, 0)) * CanvasHeight;
            float EntrySpacing = (CanvasHeight - (TopMargin + BottomMargin)) / (DisplayList.Count);
            float position = CanvasHeight/2 - TopMargin - (EntrySpacing/2);
            foreach(GameObject ob in DisplayList)
            {
                ob.transform.localPosition = new Vector3(0, position, 0);
                position -= EntrySpacing;
            }
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
                if (GlobalTools.inputsGameplay.FindAction("Submit").triggered)
                {
                    if (screenText.buttonParent != null)
                    {
                        SelectedOption = EventSystem.current.currentSelectedGameObject.name;
                    }
                    CloseScreenTex();
                    GlobalTools.Unpause();
                    Paused = false;
                }
                else if (GlobalTools.inputsGameplay.FindAction("Cancel").triggered)
                {
                    SelectedOption = null;
                    CloseScreenTex();
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
                    CloseScreenTex();
                }
            }
        }
    }
    void CloseScreenTex()
    {
        if (previewOb != null)
        {
            Destroy(previewOb);
        }
        if (screenText.buttonParent != null)
        {
            Destroy(screenText.buttonParent);
        }
        canvas.enabled = false;
        HUD.SetActive(true);
    }
}
