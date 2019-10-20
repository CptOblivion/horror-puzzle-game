using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuEnabled : MonoBehaviour
{
    public GameObject selectOnWake;

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
            }
        }
    }
}
