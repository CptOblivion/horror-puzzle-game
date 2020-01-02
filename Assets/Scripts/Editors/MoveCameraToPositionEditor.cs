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
        if (GUI.Button(new Rect(10, 10, 170, 30), "Move Camera To Position"))
        {
            MoveCameraToPosition moveCameraToPosition = (MoveCameraToPosition)target;
            moveCameraToPosition.enabled = true;
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
