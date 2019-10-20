using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public InteractTarget target;
    private void OnTriggerStay(Collider other)
    {
        InteractTarget testTarget = other.GetComponent<InteractTarget>();
        if (testTarget && testTarget.enabled)
        {
            Ray ray = new Ray(transform.position, other.transform.position - transform.position);
            if (Physics.Raycast(ray, out RaycastHit hit, 5))
            {
                if (hit.collider == other)
                {
                    //TODO: priority system
                    target = testTarget;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!GlobalTools.Paused)
        {
            target = null; //reset at the end of the frame (leave it be while paused)
        }
    }
}
