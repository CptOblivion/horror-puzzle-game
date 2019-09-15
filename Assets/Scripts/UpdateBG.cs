﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBG : MonoBehaviour
{
    public Texture2D outputTexture;
    public int repeatRender = 2;
    Camera cam;
    public bool UpdateRender = true;
    public List<CameraPosition> camVolumes = new List<CameraPosition>();
    CameraPosition currentPosition;
    public bool SilentUpdates = true;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    void Start()
    {
        cam.enabled = false;
    }

    public void CheckForUpdate()
    {
        if (!currentPosition && camVolumes.Count > 0)
        {
            currentPosition = camVolumes[camVolumes.Count - 1];
            UpdatePosition();
        }
        else if (camVolumes.Count> 0)
        {
            CameraPosition newPosition = camVolumes[camVolumes.Count - 1];
            Camera newCam = newPosition.newCamera;
            if (newCam != currentPosition.newCamera)
            {
                currentPosition = newPosition;
                UpdatePosition();
            }
        }
    }

    private void UpdatePosition()
    {
        Camera newCamera = currentPosition.newCamera;
        transform.SetPositionAndRotation(newCamera.transform.position, newCamera.transform.rotation);
        cam.fieldOfView = newCamera.fieldOfView;
        PrepUpdate();
    }

    public void PrepUpdate() //lets other objects tell this camera to update when the frame is ending
    {
        UpdateRender = true;
    }

    private void RenderBackground(bool silent = true) //actually do the update
    {
        float startTime = Time.realtimeSinceStartup;
        for (int repeat = repeatRender; repeat > 0; repeat--) cam.Render();

        outputTexture.Resize(cam.activeTexture.width, cam.activeTexture.height);
        outputTexture.Apply();
        RenderTexture.active = cam.activeTexture;
        outputTexture.ReadPixels(new Rect(0, 0, cam.activeTexture.width, cam.activeTexture.height), 0, 0);
        outputTexture.Apply();
        TextureScale.Bilinear(outputTexture, 480, 270);
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        if (!silent) Debug.Log("Updated background: " + (elapsedTime*1000) + " ms");
    }
    private void LateUpdate()
    {
        if (UpdateRender)
        {
            UpdateRender = false;
            RenderBackground(SilentUpdates);
        }
    }
}
