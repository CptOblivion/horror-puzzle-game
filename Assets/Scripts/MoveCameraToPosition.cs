using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class MoveCameraToPosition : MonoBehaviour
{
    private void Awake()
    {
        this.enabled = false;
    }
    private void OnEnable()
    {
        Camera cam = GetComponentInChildren<Camera>();
        Camera currentCam = FindObjectOfType<GlobalTools>().startingCam;
        currentCam.transform.position = cam.transform.position;
        currentCam.transform.rotation = cam.transform.rotation;
        currentCam.fieldOfView = cam.fieldOfView;
        this.enabled = false;
    }
}
