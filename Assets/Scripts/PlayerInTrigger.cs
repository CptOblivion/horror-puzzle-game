using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInTrigger : MonoBehaviour
{
    public bool playerInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GlobalTools.player)
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GlobalTools.player)
        {
            playerInTrigger = false;
        }
    }
}
