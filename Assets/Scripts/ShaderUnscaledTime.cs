using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderUnscaledTime : MonoBehaviour
{

    public bool SetUnscaledTime = false;
    public bool SetUnscaledDeltaTime = false;
    Material[] mats = new Material[] {};

    private void Awake()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        Image image = GetComponent<Image>();
        RawImage rawImage = GetComponent<RawImage>();

        if (meshRenderer)
        {
            mats = meshRenderer.materials;
        }
        else if (skinnedMeshRenderer)
        {
            mats = skinnedMeshRenderer.materials;
        }
        else if (image)
        {
            mats = new Material[] { image.material };
        }
        else if (rawImage)
        {
            mats = new Material[] { rawImage.material };
        }
        else
        {
            Debug.LogError("No renderer!", this.gameObject);
            this.enabled = false;
        }
    }

    private void LateUpdate()
    {
        foreach (Material mat in mats)
        {
            if (SetUnscaledDeltaTime) mat.SetFloat("_UnscaledDeltaTime", Time.unscaledDeltaTime);
            if (SetUnscaledTime) mat.SetFloat("_UnscaledTime", Time.unscaledTime);
        }
    }
}
