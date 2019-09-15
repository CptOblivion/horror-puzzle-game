using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAtRuntime : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
