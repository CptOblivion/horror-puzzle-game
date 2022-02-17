using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadTag : MonoBehaviour
{
  public string Tag;
  public Transform copyTransform;
  public GameObject activateObject;
  public bool OnlyOnTag = false;
  public float Delay = 0;
  void Start()
  {
    if (Delay == 0)
    {
      CheckForTag();
    }
  }
  private void Update()
  {
    if (Delay > 0)
    {
      Delay -= Time.deltaTime;
    }
    else
    {
      CheckForTag();
    }
  }

  void CheckForTag()
  {
    bool TagFound = false;
    foreach (string tag in LevelLoader.LevelTags)
    {
      if (tag == Tag)
      {
        TagFound = true;
        if (copyTransform != null)
        {
          transform.SetPositionAndRotation(copyTransform.position, copyTransform.rotation);
        }

        if (activateObject != null)
        {
          activateObject.SetActive(true);
        }

        Debug.Log("activating with tag " + Tag, gameObject);

        this.enabled = false;
        //gameObject.SetActive(false);

        break;
      }

    }
    if (!TagFound && OnlyOnTag)
      gameObject.SetActive(false);
  }
}
