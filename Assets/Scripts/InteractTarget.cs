
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTarget : MonoBehaviour
{
    [Tooltip("Object to call a function on (if blank, uses parent instead)")]
    public GameObject InteractTrigger;
    [Tooltip("Function to call")]
    public string Function = "Interact";

    public string InteractMessage = "";

    public bool OneUse = false;
    public void Interact()
    {
        if (InteractTrigger)
        {
            InteractTrigger.SendMessage(Function);
        }
        else
        {
            transform.parent.SendMessage(Function);
        }
        if (OneUse)
        {
            gameObject.SetActive(false);
        }
        if (InteractMessage != "")
        {
            ScreenText.DisplayText(InteractMessage);
        }
    }
}
