using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadTag : MonoBehaviour
{
    public string Tag;
    public Transform copyTransform;
    public GameObject activateObject;
    public bool OnlyOnTag = false;
    void Start()
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

                break;
            }

        }
        if (!TagFound && OnlyOnTag)
            gameObject.SetActive(false);
    }

}
