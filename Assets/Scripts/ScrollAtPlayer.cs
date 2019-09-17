using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollAtPlayer : MonoBehaviour
{
    public float ScrollSpeed = .005f;
    public float DepthRemove = .1f; //shrink the Z component 
    Vector2 ScrollPosition;
    Material mat;
    void Awake()
    {
        mat = GetComponent<SkinnedMeshRenderer>().material;
    }
    void Update()
    {
        Vector3 SelfPos = GlobalTools.currentCam.WorldToViewportPoint(transform.position);
        Vector3 TargetPos = GlobalTools.currentCam.WorldToViewportPoint(GlobalTools.player.transform.position);
        Vector3 ViewVec = SelfPos - TargetPos;
        Vector2 ScrollVec = new Vector2(ViewVec.x, ViewVec.y);
        float ScrollMag = ScrollVec.magnitude;
        ScrollMag = 1 - Mathf.Clamp01(ScrollMag);
        
        ScrollPosition += ScrollVec.normalized * ScrollSpeed * ScrollMag;
        mat.SetFloat("_ScrollX", ScrollPosition.x);
        mat.SetFloat("_ScrollY", ScrollPosition.y);
    }
}
