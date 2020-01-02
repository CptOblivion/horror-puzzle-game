using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitToCameras : MonoBehaviour
{
    public MoveCameraToPosition[] CameraPositions;

    private void Start()
    {
        UpdateBG.CameraWasChanged += OnCameraChange;
    }

    private void OnDestroy()
    {
        UpdateBG.CameraWasChanged -= OnCameraChange;
    }

    void OnCameraChange(Camera cam)
    {
        bool InCamera = false;
        for(int i = 0; i < CameraPositions.Length; i++)
        {
            if (CameraPositions[i].GetComponentInChildren<Camera>(true) == cam)
            {
                InCamera = true;
                break;
            }
        }

        gameObject.SetActive(InCamera);
    }
}
