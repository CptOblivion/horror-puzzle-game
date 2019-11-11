using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    public string Level;
    public bool OnCollide = true;
    public string[] Tags = { };

    private void OnTriggerEnter(Collider other)
    {
        if (OnCollide)
        {
            Collider playerCollider = GlobalTools.player.GetComponent<Collider>();
            if (other == playerCollider)
            {
                LoadLevel();
            }
        }
    }

    public void LoadLevel()
    {
        LevelLoader.LoadLevel(Level, Tags);
    }
}
