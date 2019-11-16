using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProjectorControls : MonoBehaviour
{
    public Color color = Color.white;
    Projector projector;
    public Material material;
    void Awake()
    {
        projector = GetComponent<Projector>();
        projector.material = new Material(material);
    }
    private void Start()
    {
        UpdateColor();
    }
    private void OnValidate()
    {
        if (projector && projector.material) 
        { 
            UpdateColor(); 
        }
    }

    void UpdateColor()
    {
        projector.material.SetColor("_Color", color);
        projector.material.SetVector("_ProjectorDir", transform.forward);
    }   
}
