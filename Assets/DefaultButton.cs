using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DefaultButton : MonoBehaviour
{
    public Button SelectOnWake;
    private void OnEnable()
    {
        if (SelectOnWake)
        {
            EventSystem.current.SetSelectedGameObject(SelectOnWake.gameObject);
        }
    }
}
