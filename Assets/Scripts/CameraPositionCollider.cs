using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CameraPosition cameraPosition = other.GetComponent<CameraPosition>();
        if (cameraPosition)
        {
            UpdateBG updateBG = GlobalTools.currentCam.GetComponent<UpdateBG>();
            updateBG.camVolumes.Add(cameraPosition);
            updateBG.CheckForUpdate();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        CameraPosition cameraPosition = other.GetComponent<CameraPosition>();
        if (cameraPosition)
        {
            UpdateBG updateBG = GlobalTools.currentCam.GetComponent<UpdateBG>();
            bool removed = updateBG.camVolumes.Remove(cameraPosition);
            if (removed)
            {
                updateBG.CheckForUpdate();
            }
            else
            {
                Debug.Log("not in list! (shouldn't be possible)");
            }
        }
    }
}
