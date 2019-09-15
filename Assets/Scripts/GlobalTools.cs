using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTools : MonoBehaviour
{
    public Camera startingCam;
    public int Framerate = 24;
    public bool vSync = true;
    public static Camera currentCam;
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
}
