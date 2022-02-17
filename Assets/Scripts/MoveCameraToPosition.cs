using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class MoveCameraToPosition : MonoBehaviour
{
  private void Awake()
  {
    this.enabled = false;
  }
#if UNITY_EDITOR
  public void MoveGameCamToCam()
  {
    Camera cam = GetComponentInChildren<Camera>();
    Camera currentCam = FindObjectOfType<GlobalTools>().startingCam;
    currentCam.transform.position = cam.transform.position;
    currentCam.transform.rotation = cam.transform.rotation;
    currentCam.fieldOfView = cam.fieldOfView;
    foreach (Camera newCam in currentCam.GetComponentsInChildren<Camera>())
    {
      if (newCam != currentCam)
      {
        newCam.fieldOfView = currentCam.fieldOfView;
        break;
      }
    }
    this.enabled = false;
  }
  public void MoveViewToCam()
  {
    Camera cam = GetComponentInChildren<Camera>();
    SceneView.lastActiveSceneView.rotation = cam.transform.rotation;
    SceneView.lastActiveSceneView.pivot = cam.transform.position + cam.transform.forward * SceneView.lastActiveSceneView.cameraDistance;
    //SceneView.lastActiveSceneView.camera.transform.position = cam.transform.position;
    //SceneView.lastActiveSceneView.camera.transform.rotation = cam.transform.rotation;
  }
  public void MoveCamToView()
  {
    Camera cam = GetComponentInChildren<Camera>();
    /*
    Selection.activeGameObject = cam.gameObject;
    SceneView.lastActiveSceneView.AlignWithView();
    Selection.activeGameObject = gameObject;
    */

    //TODO: undo functionality
    Undo.RecordObject(cam.transform, "Align camera to view");
    cam.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
    cam.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
  }
#endif
}
