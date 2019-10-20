using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public GameObject selectOnWake;
    public AudioClip moveSound;
    GameObject lastSelected;

    bool WaitAFrame = false;
    private void OnEnable()
    {
        WaitAFrame = true;
    }

    private void LateUpdate()
    {

        if (WaitAFrame)
        {
            WaitAFrame = false;
            if (selectOnWake)
            {
                EventSystem.current.SetSelectedGameObject(selectOnWake);
                lastSelected = EventSystem.current.currentSelectedGameObject;
            }
        }
        else if (lastSelected != EventSystem.current.currentSelectedGameObject)
        {
            PlaySFX.Play(moveSound);
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
}
