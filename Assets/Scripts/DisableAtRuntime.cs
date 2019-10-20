﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAtRuntime : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
