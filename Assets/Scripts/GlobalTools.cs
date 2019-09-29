using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class GlobalTools : MonoBehaviour
{
    public static bool Paused = false;
    public static bool WasPaused = false;
    public Camera startingCam;
    public Canvas overlayCanvas;
    public int Framerate = 24;
    public bool vSync = true;
    public static Camera currentCam;
    public static GameObject player;
    public static GlobalTools globalTools;
    public InputActionAsset inputActionAsset;
    public static InputActionAsset inputActions;
    public static InputActionMap inputsGameplay;
    public static InputActionMap inputsMenus;

    PlayerInput playerInput;
    public enum LayerMasks { Default, TransparentFX, IngoreRaycast, Blank0, Water, UI, Blank1, Blank2, PostProcessing, RenderOBJ, RenderBG, Clickable, ClickOcclude }
    private void Awake()
    {
        globalTools = this;
        currentCam = startingCam;
        playerInput = this.GetComponent<PlayerInput>();
        inputActions = this.inputActionAsset;
    }
    void Start()
    {
        if (!vSync)
        {
            QualitySettings.vSyncCount = 0;
        }
        Application.targetFrameRate = Framerate;

        inputsGameplay = inputActions.GetActionMap("Gameplay");
        inputsMenus = inputActions.GetActionMap("Menus");
        inputsGameplay.Enable();
        //inputsMenus.Disable();
    }
    private void LateUpdate()
    {
        if (WasPaused)
        {
            WasPaused = false;
        }
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
        //globalTools.playerInput.SwitchCurrentActionMap("Menus");
        inputsGameplay.Disable();
        //inputsMenus.Enable();
    }
    public static void Unpause()
    {
        Time.timeScale = 1;
        Paused = false;
        GlobalTools.WasPaused = true;
        //globalTools.playerInput.SwitchCurrentActionMap("Gameplay");
        inputsGameplay.Enable();
        //inputsMenus.Disable();
    }
}
