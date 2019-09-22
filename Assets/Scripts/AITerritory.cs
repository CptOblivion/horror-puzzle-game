using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITerritory : MonoBehaviour
{
    //[HideInInspector]
    public bool PlayerInTerritory = false;
    int Intersections = 0;
    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GlobalTools.player)
        {
            Intersections += 1;
            PlayerInTerritory = true;
            //Debug.Log(Intersections);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GlobalTools.player)
        {
            Intersections -= 1;
            if (Intersections <= 0)
            {
                PlayerInTerritory = false;
                //Debug.Log("leaving " + name);
            }
        }
    }
}
