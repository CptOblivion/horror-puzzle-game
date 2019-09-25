using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BGFrame
{
    [Range(1, 4)]
    public int FrameNumber;
    public float FrameTime = 0.1f;
}
public class GlobalTools : MonoBehaviour
{
    public static bool Paused = false;
    public Camera startingCam;
    public Canvas overlayCanvas;
    public int Framerate = 24;
    public bool vSync = true;
    public static Camera currentCam;
    public static GameObject player;
    public static GlobalTools globalTools;
    public enum LayerMasks { Default, TransparentFX, IngoreRaycast, Blank0, Water, UI, Blank1, Blank2, PostProcessing, RenderOBJ, RenderBG, Clickable, ClickOcclude }
    private void Awake()
    {
        globalTools = this;
        currentCam = startingCam;
    }
    void Start()
    {
        if (!vSync)
        {
            QualitySettings.vSyncCount = 0;
        }
        Application.targetFrameRate = Framerate;
    }
    public static void SnapToGround(CharacterController characterController, float GroundSnapDistance)
    {
        if (!characterController.isGrounded)
        {
            float feetDist = (characterController.height / 2) - characterController.radius;
            Vector3 feetPos = characterController.transform.position + new Vector3(0, -feetDist, 0);
            RaycastHit hit;
            //float safetyMargin = .01f;
            if (Physics.SphereCast(feetPos, characterController.radius, Vector3.down, out hit, GroundSnapDistance))
            {
                //float angleMargin = .5f;
                if (Vector3.Dot(hit.normal, characterController.transform.up) > Mathf.Sin(Mathf.Deg2Rad * characterController.slopeLimit))
                {
                    float dropDistance = hit.distance;
                    characterController.transform.position = characterController.transform.position + new Vector3(0, -(dropDistance - characterController.skinWidth), 0);

                }
            }
        }

    }

    public static void Pause()
    {
        Time.timeScale = 0;
        Paused = true;
    }
    public static void Unpause()
    {
        Time.timeScale = 1;
        Paused = false;
    }
}
