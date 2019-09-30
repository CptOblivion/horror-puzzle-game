using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBG : MonoBehaviour
{
    public Texture2D[] outputTextures = new Texture2D[4];
    public int repeatRender = 2;
    public bool UpdateRender = false;
    public List<CameraPosition> camVolumes = new List<CameraPosition>();
    public bool SilentUpdates = true;
    public MeshRenderer drawBG;

    Texture2D currentOutputTexture;
    Camera camBG;
    Camera camOB;
    CameraPosition currentPosition;
    int FrameCount;
    BGFrame[] frameTiming;
    int CurrentFrame;
    float CurrentFrameTiming = 0;
    Animation[] AnimObs;

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
        CameraData cameraData = newCamera.GetComponent<CameraData>();
        transform.SetPositionAndRotation(newCamera.transform.position, newCamera.transform.rotation);
        camBG.fieldOfView = newCamera.fieldOfView;
        frameTiming = cameraData.frameTiming;
        //just in case we've left FrameCount at higher than 1 without setting a framerate, let's not render frames that won't be used
        if (frameTiming.Length > 0) FrameCount = cameraData.FrameCount;
        else FrameCount = 1;
        AnimObs = cameraData.AnimObs;
        CurrentFrame = 1;
        if (frameTiming.Length > 0)
        {
            CurrentFrameTiming = frameTiming[0].FrameTime;
            currentOutputTexture = outputTextures[(frameTiming[0].FrameNumber - 1) % FrameCount];
            drawBG.material.SetTexture("_Color", currentOutputTexture);
        }

        PrepRender();
    }
    private void UpdateAnims(float t)
    {
        foreach (Animation anim in AnimObs)
        {
            string clip = anim.clip.name;
            anim[clip].speed = 0;
            anim[clip].time = t;
            anim.Play();
            anim.Sample();
        }

    }

    public void PrepRender() //lets other objects tell this camera to update when the frame is ending
    {
        //Debug.Log("Rendering");
        UpdateRender = true;
    }

    private void RenderBackground(int repeat = 1) //actually do the update
    {
        //Debug.Log("RenderingIteration");
        for (; repeat > 0; repeat--)
        {
            camBG.Render();
        }

        if (camBG.targetTexture.width != camOB.targetTexture.width)
        {
            currentOutputTexture.Resize(camBG.activeTexture.width, camBG.activeTexture.height);
            currentOutputTexture.Apply();
            RenderTexture.active = camBG.activeTexture;
            currentOutputTexture.ReadPixels(new Rect(0, 0, camBG.activeTexture.width, camBG.activeTexture.height), 0, 0);
            currentOutputTexture.Apply();
            TextureScale.Bilinear(currentOutputTexture, camOB.targetTexture.width, camOB.targetTexture.height);
        }
        else
        {
            RenderTexture.active = camBG.activeTexture;
            currentOutputTexture.ReadPixels(new Rect(0, 0, camBG.activeTexture.width, camBG.activeTexture.height), 0, 0);
            currentOutputTexture.Apply();
        }
    }
    private void Awake()
    {
        camBG = GetComponent<Camera>();
        foreach (Camera testcam in transform.GetComponentsInChildren<Camera>())
        {
            if (testcam != camBG)
            {
                camOB = testcam;
                break;
            }
        }
        currentOutputTexture = outputTextures[0];
    }
    void Start()
    {

        camBG.enabled = false;
        RenderBackground();
    }
    private void LateUpdate()
    {
        if (UpdateRender)
        {
            UpdateRender = false;

            float startTime = Time.realtimeSinceStartup;
            for (int i = 0; i < FrameCount; i++)
            {
                UpdateAnims(i);
                currentOutputTexture = outputTextures[i];
                if (i == 0) RenderBackground(repeatRender);
                RenderBackground();

            }
            UpdateAnims(0); //reset all the animations to the first frame in case they're in the background of another angle
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            if (!SilentUpdates) Debug.Log("Updated background: " + (elapsedTime * 1000) + " ms");
        }

        //playing back pre-rendered frames (animated or still)
        if (frameTiming.Length > 0) //don't animate if there's no frameTiming
        {
            CurrentFrameTiming -= Time.deltaTime;
            if (CurrentFrameTiming <= 0)
            {
                CurrentFrame += 1;
                if (frameTiming.Length == 1) //if there's just one entry in frametiming, we're just setting a flat framerate
                {
                    currentOutputTexture = outputTextures[(CurrentFrame-1) % FrameCount];
                }
                else //multiple entries means we're manually controlling frame timing
                {
                    //for readability's sake the frames in the interface are 1-4, but for array entries we need 0-3 so don't forget to subtract 1!
                    BGFrame activeFrame = frameTiming[(CurrentFrame - 1) % frameTiming.Length];
                    CurrentFrameTiming = activeFrame.FrameTime;

                    //we're still using %FrameCount here to avoid drawing frames to the screen that haven't been rendered for this camera
                    //in case in the scene I set framecount to 2 and accidentally reference frames 3 or 4
                    //also I just feel like using % as many times as possible so I have a chance of remembering to use it later
                    currentOutputTexture = outputTextures[(activeFrame.FrameNumber-1) % FrameCount];
                }

                //we'll need to make sure we're setting the right texture here

                drawBG.material.SetTexture("_Color", currentOutputTexture);
                //drawBG.material.mainTexture = currentOutputTexture;
            }
        }
        else
        {
            currentOutputTexture = outputTextures[0];
            drawBG.material.SetTexture("_Color", currentOutputTexture);
        }
    }
}
