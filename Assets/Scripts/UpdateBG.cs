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
    public RawImage drawBG;
    public Material PixelGrid;
    public int BGScale = 3; //don't use after the first time this script is implemented! (Rendertexture size is locked in at that point, from then on use BGScale.width / OBCam.TargetTexture.width instead)
    public Shader blitShader;
    public bool ScaleWithShader = true;

    public static RenderTexture BGTexture;
    public static RenderTexture BGIntermediary;
    Texture2D currentOutputTexture;
    Camera camBG;
    public Camera camOB;
    CameraPosition currentPosition;
    int FrameCount;
    BGFrame[] frameTiming;
    int CurrentFrame;
    float CurrentFrameTiming = 0;
    Animation[] AnimObs;
    int WaitABit = 2;

    private void Awake()
    {
        camBG = GetComponent<Camera>();
        if (BGTexture == null)
        {
            int[] texSize = { camOB.targetTexture.width * BGScale, camOB.targetTexture.height * BGScale };
            Debug.Log("Background Render Texture Size: " + texSize[0] + " x " + texSize[1]);
            BGTexture = new RenderTexture(texSize[0], texSize[1], 0);
            BGTexture.antiAliasing = 1;
            BGTexture.filterMode = FilterMode.Point;
        }
        if (BGIntermediary == null)
        {
            BGIntermediary = new RenderTexture(camOB.targetTexture.width, camOB.targetTexture.height, 0);
            BGTexture.antiAliasing = 1;
            BGTexture.filterMode = FilterMode.Point;

        }
        /*
        foreach (Camera testcam in transform.GetComponentsInChildren<Camera>())
        {
            if (testcam != camBG)
            {
                camOB = testcam;
                break;
            }
        }
        */
        currentOutputTexture = outputTextures[0];
    }
    void Start()
    {
        camBG.targetTexture = BGTexture;
        //camOB.clearFlags = CameraClearFlags.Color;

        camBG.enabled = false;
        camOB.enabled = false;
        //RenderBackground();
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
                if (i == 0 && repeatRender > 1) RenderBackground(repeatRender - 1);
                RenderBackground();

            }
            currentOutputTexture = outputTextures[0];
            UpdateAnims(0); //reset all the animations to the first frame in case they're in the background of another angle
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            if (!SilentUpdates) Debug.Log("Updated background: " + (elapsedTime * 1000) + " ms");
        }

        //playing back pre-rendered frames (animated or still)
        if (frameTiming == null)
        {
            //Debug.Log("waiting (Check UpdateBG if this shows up! Should be a black screen while it's happening)");
            /*
            foreach (Texture2D tex in outputTextures)
            {
                Color[] colors = new Color[tex.width * tex.height];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = Color.black;
                }
                tex.SetPixels(colors);
                tex.Apply();
            }
            */
        }
        else if (frameTiming.Length > 0) //don't animate if there's no frameTiming
        {
            CurrentFrameTiming -= Time.deltaTime;
            if (CurrentFrameTiming <= 0)
            {
                CurrentFrame += 1;
                if (frameTiming.Length == 1) //if there's just one entry in frametiming, we're just setting a flat framerate
                {
                    currentOutputTexture = outputTextures[(CurrentFrame - 1) % FrameCount];
                }
                else //multiple entries means we're manually controlling frame timing
                {
                    //for readability's sake the frames in the interface are 1-4, but for array entries we need 0-3 so don't forget to subtract 1!
                    BGFrame activeFrame = frameTiming[(CurrentFrame - 1) % frameTiming.Length];
                    CurrentFrameTiming = activeFrame.FrameTime;

                    //we're still using %FrameCount here to avoid drawing frames to the screen that haven't been rendered for this camera
                    //in case in the scene I set framecount to 2 and accidentally reference frames 3 or 4
                    //also I just feel like using % as many times as possible so I have a chance of remembering to use it later
                    currentOutputTexture = outputTextures[(activeFrame.FrameNumber - 1) % FrameCount];
                }

                //we'll need to make sure we're setting the right texture here
                //drawBG.texture = currentOutputTexture;
                Graphics.Blit(currentOutputTexture, camOB.targetTexture);
            }
            Graphics.Blit(currentOutputTexture, camOB.targetTexture);
        }
        else
        {
            currentOutputTexture = outputTextures[0];
            //drawBG.texture = currentOutputTexture;
            Graphics.Blit(currentOutputTexture, camOB.targetTexture);
        }
        if (WaitABit > 0)
        {
            WaitABit --;
        }
        else
        {
            camOB.enabled = true;
        }
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
        else
        {
            //Debug.Log("No active camera volumes (remaining on current camera, just a heads up)");
        }
    }

    private void UpdatePosition()
    {
        Camera newCamera = currentPosition.newCamera;
        CameraData cameraData = newCamera.GetComponent<CameraData>();
        transform.SetPositionAndRotation(newCamera.transform.position, newCamera.transform.rotation);
        camBG.fieldOfView = newCamera.fieldOfView;
        camOB.fieldOfView = newCamera.fieldOfView;
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
            drawBG.texture = currentOutputTexture;
            //drawBG.material.SetTexture("_Color", currentOutputTexture);
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

    public void PrepRender() //lets other objects tell this camera to update at the end of the frame
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
            if (ScaleWithShader)
            {
                Material mat = new Material(blitShader);
                mat.SetInt("TargetWidth", BGTexture.width);
                mat.mainTexture = camOB.targetTexture;
                Graphics.Blit(BGTexture, BGIntermediary, mat);

                RenderTexture.active = BGIntermediary;
                currentOutputTexture.ReadPixels(new Rect(0, 0, BGIntermediary.width, BGIntermediary.height), 0, 0);
                currentOutputTexture.Apply();
            }
            else
            {
                currentOutputTexture.Resize(BGTexture.width, BGTexture.height);
                //currentOutputTexture.Apply();
                RenderTexture.active = BGTexture;
                currentOutputTexture.ReadPixels(new Rect(0, 0, BGTexture.width, BGTexture.height), 0, 0);

                currentOutputTexture.Apply();
                DownscaleTex(currentOutputTexture, BGTexture.width/camOB.targetTexture.width);
            }
        }
        else
        {
            RenderTexture.active = camBG.activeTexture;
            currentOutputTexture.ReadPixels(new Rect(0, 0, camBG.activeTexture.width, camBG.activeTexture.height), 0, 0);
            currentOutputTexture.Apply();
        }
    }
    void DownscaleTex(Texture2D tex, int factor)
    {
        //TODO: Add threading (slice image into sections equal to number of cores, process each slice in its own thread)
        int[] size = new int[] { tex.width / factor, tex.height / factor };
        Color[] colorArray = new Color[size[0]*size[1]];
        for(int y = 0; y < size[1]; y++)
        {
            for (int x = 0; x < size[0]; x++)
            {
                Color[] PixelBlock = tex.GetPixels(x * factor, y * factor, factor, factor);
                Color PixelColor = Color.black;
                foreach (Color pixel in PixelBlock)
                {
                    PixelColor += pixel;
                }
                PixelColor /= factor * factor;
                colorArray[size[0]*y+x] = PixelColor;
            }
        }
        tex.Resize(size[0], size[1]);
        tex.SetPixels(colorArray);
        tex.Apply();

    }
}
