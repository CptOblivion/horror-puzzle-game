using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTools : MonoBehaviour
{
    public Camera startingCam;
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
            if (Physics.SphereCast(feetPos, characterController.radius, Vector3.down, out hit, GroundSnapDistance))
            {
                float dropDistance = hit.distance;
                characterController.transform.position = characterController.transform.position + new Vector3(0, -(dropDistance - characterController.skinWidth), 0);
            }
        }

    }
}
