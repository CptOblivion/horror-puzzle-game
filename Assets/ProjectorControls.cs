using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProjectorControls : MonoBehaviour
{
    public Color color = Color.white;
    Projector projector;
    public Material material;
    public bool SharpFalloff = false;
    [Range(-1,1)]
    public float BackfaceOffset;
    public Texture2D Cookie;
    public Texture2D Falloff;
    void Awake()
    {
        projector = GetComponent<Projector>();
        projector.material = new Material(material);
        
    }
    private void Start()
    {
        UpdateMaterial();
    }
    private void OnValidate()
    {
        if (projector && projector.material) 
        { 
            UpdateMaterial(); 
        }
    }

    void UpdateMaterial()
    {
        projector.material.SetColor("_Color", color);
        projector.material.SetVector("_ProjectorDir", transform.forward);
        projector.material.SetFloat("_BackfaceOffset", BackfaceOffset);
        if (Cookie != null)projector.material.SetTexture("_ShadowTex", Cookie);
        if (Falloff != null)projector.material.SetTexture("_FalloffTex", Falloff);
    }   
}
