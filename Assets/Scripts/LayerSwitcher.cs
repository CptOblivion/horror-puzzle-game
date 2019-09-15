using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSwitcher : MonoBehaviour
{
    //this should move to a static component everything can reference
    public GlobalTools.LayerMasks PhysicsLayer = GlobalTools.LayerMasks.Clickable;
    public GlobalTools.LayerMasks RenderLayer = GlobalTools.LayerMasks.RenderBG;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void LateUpdate()
    {
        //set layer to render layer (after physics are done)
        gameObject.layer = (int)RenderLayer;
    }
    private void OnGUI()
    {
        //set layer to physics layer (in preparation for physics next frame)
        gameObject.layer = (int)PhysicsLayer;
    }
}
