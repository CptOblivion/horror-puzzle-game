using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraData : MonoBehaviour
{
    [Range(1, 4)]
    public int FrameCount = 1;
    public BGFrame[] frameTiming;
    public Animation[] AnimObs;

}
