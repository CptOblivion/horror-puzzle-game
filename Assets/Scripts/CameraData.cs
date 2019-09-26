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
public class CameraData : MonoBehaviour
{
    [Range(1, 4)]
    public int FrameCount = 1;
    public BGFrame[] frameTiming;
    public Animation[] AnimObs;

}
