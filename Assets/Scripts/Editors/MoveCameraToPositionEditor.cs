#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveCameraToPosition))]
public class MoveCameraToPositionEditor : Editor
{
    void OnSceneGUI()
    {
        Handles.BeginGUI();
        if (GUI.Button(new Rect(10, 10, 200, 25), "Move Game Cam To Cam Position"))
        {
            MoveCameraToPosition moveCameraToPosition = (MoveCameraToPosition)target;
            moveCameraToPosition.MoveGameCamToCam();
            //moveCameraToPosition.enabled = true;
        }
        if (!SceneView.lastActiveSceneView.orthographic && GUI.Button(new Rect (10, 40, 180, 25), "Move View To Cam Position"))
        {

            MoveCameraToPosition moveCameraToPosition = (MoveCameraToPosition)target;
            moveCameraToPosition.MoveViewToCam();
        }
        if (!SceneView.lastActiveSceneView.orthographic && GUI.Button(new Rect(10, 70, 190, 25), "Align Cam Position With View"))
        {

            MoveCameraToPosition moveCameraToPosition = (MoveCameraToPosition)target;
            moveCameraToPosition.MoveCamToView();
        }


        Handles.EndGUI();
    }
    /*
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Move Camera To Position"))
        {
            MoveCameraToPosition moveCameraToPosition = (MoveCameraToPosition)target;
            moveCameraToPosition.enabled = true;
        }
    }
    */
}
#endif
