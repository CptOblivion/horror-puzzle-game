using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadTag : MonoBehaviour
{
    public string Tag;
    public Transform copyTransform;
    public GameObject activateObject;
    void Start()
    {
        foreach (string tag in LevelLoader.LevelTags)
        {
            if (tag == Tag)
            {
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
    }

}
