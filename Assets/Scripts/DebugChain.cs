using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChain : MonoBehaviour
{
    public List<Vector3> debugPositions = new List<Vector3>();

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(.1f, 0, 0));
        for (int i = 0; i < debugPositions.Count; i++)
        {
            Vector3 lastPos;
            if (i == 0)
            {
                lastPos = transform.position;
            }
            else
            {
                lastPos = debugPositions[i - 1];
            }
            Vector3 newPos = debugPositions[i];
            Debug.DrawLine(lastPos, newPos);
            Debug.DrawLine(newPos, newPos + new Vector3(.1f, 0, 0));
        }
    }
}
