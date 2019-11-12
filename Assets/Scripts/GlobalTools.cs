using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GlobalTools : MonoBehaviour
{

    public Camera startingCam;

    //ini settings (hardcoded for now)
    public int DefaultFramerate = 60;
    public int DefaultVSync = 0;
    public bool DefaultPauseOnFocusLoss = true;
    public bool DefaultBarrelDistortion = true;
    public float DefaultVolumeMaster = -10;
    public float DefaultVolumeMusic = 0;
    public float DefaultVolumeSound = 0;

    public static bool Paused = false;
    public static bool WasPaused = false;
    public static bool StartupFinished = true; //track if the game has started or if this is just a scene change
    public static Camera currentCam;
    public static GameObject player;
    public static GlobalTools globalTools;
    public InputActionAsset inputActionAsset;
    public static InputActionAsset inputActions;
    public static InputActionMap inputsGameplay;
    //public static InputActionMap inputsMenus;

    //PlayerInput playerInput;
    public enum LayerMasks { Default, TransparentFX, IngoreRaycast, Blank0, Water, UI, Blank1, Blank2, PostProcessing, RenderOBJ, RenderBG, Clickable, ClickOcclude }
    private void Awake()
    {
        globalTools = this;
        currentCam = startingCam;
        //playerInput = this.GetComponent<PlayerInput>();
        inputActions = this.inputActionAsset;
        Unpause();
        Cursor.visible = false;

        if (!StartupFinished)
        {
            if (!PlayerPrefs.HasKey("VSync"))
                PlayerPrefs.SetInt("VSync", DefaultVSync);
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync");

            if (!PlayerPrefs.HasKey("Target Framerate"))
                PlayerPrefs.SetInt("Target Framerate", DefaultFramerate);
            Application.targetFrameRate = PlayerPrefs.GetInt("Target Framerate");

            if (!PlayerPrefs.HasKey("Fullscreen mode"))
                PlayerPrefs.SetInt("Fullscreen mode", (int)FullScreenMode.Windowed);
            Screen.fullScreenMode = (FullScreenMode)PlayerPrefs.GetInt("Window mode");

            if (!PlayerPrefs.HasKey("Pause on focus loss"))
                PlayerPrefs.SetInt("Pause on focus loss", DefaultPauseOnFocusLoss ? 1 : 0);

            if (!PlayerPrefs.HasKey("Barrel distortion"))
                PlayerPrefs.SetInt("Barrel distortion", DefaultBarrelDistortion ? 1 : 0);

            //TODO: link this up to something, add menu controls
            if (!PlayerPrefs.HasKey("Volume master"))
                PlayerPrefs.SetFloat("Volume master", DefaultVolumeMaster);
            if (!PlayerPrefs.HasKey("Volume music"))
                PlayerPrefs.SetFloat("Volume music", DefaultVolumeMusic);
            if (!PlayerPrefs.HasKey("Volume sound"))
                PlayerPrefs.SetFloat("Volume sound", DefaultVolumeSound);
        }
    }
    void Start()
    {
        if (!StartupFinished)
        {
            StartupFinished = true;
        }


        inputsGameplay = inputActions.FindActionMap("Gameplay");
        //inputsMenus = inputActions.FindActionMap("Menus");
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
    private void OnApplicationFocus(bool focus)
    {
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager && !focus)
        {
            if (PlayerPrefs.GetInt("Pause on focus loss") == 1)
            {
                if (!GlobalTools.Paused)
                {
                    inventoryManager.OpenInventory();
                }
            }
            else
            {
                Debug.Log("Focus lost, carrying on nonetheless!",globalTools);
            }
        }
    }
    public static void SnapToGround(CharacterController characterController, float GroundSnapDistance)
    {
        if (!characterController.isGrounded)
        {
            float feetDist = (characterController.height / 2) - characterController.radius;
            //Vector3 feetPos = characterController.transform.position + new Vector3(0, -feetDist, 0);
            Vector3 feetPos = characterController.transform.position - characterController.transform.up * feetDist;// new Vector3(0, -feetDist, 0);
            //float safetyMargin = .01f;
            if (Physics.SphereCast(feetPos, characterController.radius, -characterController.transform.up, out RaycastHit hit, GroundSnapDistance))
            {
                Debug.DrawLine(feetPos, hit.point);
                //float angleMargin = .5f;
                float HitSlopeAngle = Vector3.Dot(hit.normal, characterController.transform.up);
                if (HitSlopeAngle > Mathf.Sin(Mathf.Deg2Rad * characterController.slopeLimit) && hit.point.y < feetPos.y)
                {
                    float dropDistance = hit.distance;
                    //characterController.transform.position = characterController.transform.position + new Vector3(0, -(dropDistance - characterController.skinWidth), 0);
                    characterController.transform.position = characterController.transform.position - characterController.transform.up * (dropDistance - characterController.skinWidth);
                }
            }
        }

    }


    public static void Pause()
    {
        Time.timeScale = 0;
        Paused = true;
        //globalTools.playerInput.SwitchCurrentActionMap("Menus");
        //inputsGameplay.Disable();
        //inputsMenus.Enable();
    }
    public static void Unpause()
    {
        Time.timeScale = 1;
        Paused = false;
        GlobalTools.WasPaused = true;
        //globalTools.playerInput.SwitchCurrentActionMap("Gameplay");
        //inputsGameplay.Enable();
        //inputsMenus.Disable();
    }

    public string TimeSinceLastSave()
    {
        if (SaveManager.LastSaveTime == null)
        {
            return "You have not saved!";
        }
        else
        {
            int MinutesSinceLastSave = (int)((Time.unscaledTime - SaveManager.LastSaveTime) / 60);
            return "It has been " + MinutesSinceLastSave + " since your last save.";
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void QuitToMenu()
    {
        //should add an unsaved game warning popup or something
        StartupFinished = false; //let's not re-run those splash screens, eh?
        InventoryManager.Inventory = null; //we'll take the inventory from scene to scene, but not to the main menu.


        SceneManager.LoadScene("MainMenu");
    }

    public void ToggleFullscreen()
    {

        if (Screen.fullScreenMode == FullScreenMode.Windowed)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;
        PlayerPrefs.SetInt("Fullscreen mode", (int)Screen.fullScreenMode);

    }

    public void NewGame()
    {
        string[] levelTags = { "newgame" };
        SaveManager.ClearSaveData();
        LevelLoader.LoadLevel("Apartment", levelTags);
    }
    public void SaveGame()
    {
        SaveManager.SaveSaveFile();
    }

    public void LoadGame()
    {
        SaveManager.LoadSaveFile();
        LevelLoader.LoadLevel(SaveManager.scene, new string[] { SaveManager.SaveLocation });
    }
    public void ChairSave()
    {
        SaveManager.SaveLocation = "SavedAtChair";
        SaveGame();
        ScreenText.DisplayText("Game Saved");
    }
}
