using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class NPCMovingWaypoint
{
    public Vector3 Location;
    public Vector3 Rotation;
    public float Time;
    public UnityEvent Event;
    
    public bool ShowEvents = false;
}
[RequireComponent(typeof(AudioSource))]
public class NPCMoving : MonoBehaviour
{
    public NPCMovingWaypoint[] Waypoints = new NPCMovingWaypoint[0];
    public float StartDelay = 0;
    public bool Loop = true;

    int CurrentWaypointIndex = 0;
    float WaypointTime = 0;
    Vector3 LastPosition = Vector3.zero;
    Quaternion LastRotation = Quaternion.identity;

    void Start()
    {
        LastPosition = transform.localPosition;
        LastRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (StartDelay == 0)
        {
            StartDelay = -1;
            DoCommand();
        }
        else if (StartDelay > 0)
        {
            StartDelay -= Time.deltaTime;
            if (StartDelay <= 0)
            {
                DoCommand();
            }
        }
        else
        {
            NPCMovingWaypoint currentWaypoint = Waypoints[CurrentWaypointIndex];

            float WaypointTimeNormalized = 1f; //initialize at 1 so if time is 0 it snaps to the next waypoint
            if (currentWaypoint.Time != 0) { WaypointTimeNormalized = WaypointTime / currentWaypoint.Time; } //if time isn't 0, interpolate
            
            transform.localPosition = Vector3.Lerp(LastPosition, currentWaypoint.Location, WaypointTimeNormalized);
            transform.localRotation = Quaternion.Lerp(LastRotation, Quaternion.Euler(currentWaypoint.Rotation), WaypointTimeNormalized);
            WaypointTime += Time.deltaTime;
            if (WaypointTime > currentWaypoint.Time) { NextWaypoint(); }
        }
    }

    void NextWaypoint()
    {
        LastPosition = Waypoints[CurrentWaypointIndex].Location;
        LastRotation = Quaternion.Euler(Waypoints[CurrentWaypointIndex].Rotation);
        CurrentWaypointIndex += 1;
        if (CurrentWaypointIndex > Waypoints.Length - 1) { CurrentWaypointIndex = 0; }
        WaypointTime = 0;
        DoCommand();
    }

    void DoCommand()
    {
        if (Waypoints[CurrentWaypointIndex].Event != null)
        {
            Waypoints[CurrentWaypointIndex].Event.Invoke();
        }
    }
}
