using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
  public string Level;
  public bool OnCollide = true;
  public string[] LevelTags = { };
  public string[] SaveTags = { };

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
    for (int i = 0; i < SaveTags.Length; i++)
    {
      SaveManager.SetBool(SaveTags[i], true);
    }
    LevelLoader.LoadLevel(Level, LevelTags);
  }
}
