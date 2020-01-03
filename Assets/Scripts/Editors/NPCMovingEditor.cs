#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCMoving))]
public class NPCMovingEditor : Editor
{
    SerializedProperty Loop;
    SerializedProperty StartDelay;
    SerializedProperty Waypoints;
    private void OnEnable()
    {
        Loop = serializedObject.FindProperty("Loop");
        StartDelay = serializedObject.FindProperty("StartDelay");
        Waypoints = serializedObject.FindProperty("Waypoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(Loop);
        EditorGUILayout.PropertyField(StartDelay);
        EditorGUILayout.Space();

        Rect WaypointList = EditorGUILayout.BeginVertical("Box");
        {
            if (GUILayout.Button("V Insert Waypoint V"))
            {
                Waypoints.InsertArrayElementAtIndex(0);
            }

            for (int i = 0; i < Waypoints.arraySize; i++)
            {
                var CurrentWaypoint = Waypoints.GetArrayElementAtIndex(i);
                Rect WaypointContents = EditorGUILayout.BeginVertical("Box");
                {

                    //location line
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(CurrentWaypoint.FindPropertyRelative("Location"));
                        if (GUILayout.Button("Copy Location"))
                        {
                            NPCMoving script = (NPCMoving)target;
                            CurrentWaypoint.FindPropertyRelative("Location").vector3Value = script.transform.localPosition;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    //rotation line
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(CurrentWaypoint.FindPropertyRelative("Rotation"));
                        if (GUILayout.Button("Copy Rotation"))
                        {
                            NPCMoving script = (NPCMoving)target;
                            CurrentWaypoint.FindPropertyRelative("Rotation").vector3Value = script.transform.localEulerAngles;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    //time, sound
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUIUtility.labelWidth = 45;
                        EditorGUILayout.PropertyField(CurrentWaypoint.FindPropertyRelative("Time"));
                        EditorGUIUtility.labelWidth = 80;
                        EditorGUILayout.PropertyField(CurrentWaypoint.FindPropertyRelative("ShowEvents"));
                        EditorGUIUtility.labelWidth = 0;
                    }
                    EditorGUILayout.EndHorizontal();

                    NPCMoving npcMoving = (NPCMoving)serializedObject.targetObject;
                    //if (CurrentWaypoint.FindPropertyRelative("ShowEvents").boolValue) EditorGUILayout.PropertyField(CurrentWaypoint.FindPropertyRelative("Event"));
                    if (CurrentWaypoint.FindPropertyRelative("ShowEvents").boolValue || npcMoving.Waypoints[i].Event.GetPersistentEventCount() > 0) EditorGUILayout.PropertyField(CurrentWaypoint.FindPropertyRelative("Event"));

                    //add and remove buttons
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Remove Waypoint"))
                        {
                            Waypoints.DeleteArrayElementAtIndex(i);
                            i--;
                        }
                        if (GUILayout.Button("V Insert Waypoint V"))
                        {
                            Waypoints.InsertArrayElementAtIndex(i + 1);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif