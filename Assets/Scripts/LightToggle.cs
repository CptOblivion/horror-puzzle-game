using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightToggle : MonoBehaviour
{
    public Light attachedLight;
    public bool SwitchedOn = true;
    void Start()
    {
        if (attachedLight) attachedLight.gameObject.SetActive(SwitchedOn);
        if (SwitchedOn) gameObject.GetComponent<MeshRenderer>().material.SetFloat("_power", 1);
        else gameObject.GetComponent<MeshRenderer>().material.SetFloat("_power", 0);
    }
}
