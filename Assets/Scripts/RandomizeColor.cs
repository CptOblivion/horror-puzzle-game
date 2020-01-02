using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeColor : MonoBehaviour
{
    public int MaterialSlot;
    public Color[] Colors;

    private void Start()
    {
        ChangeColor();
    }

    public void ChangeColor()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        mesh.materials[MaterialSlot].color = Colors[Random.Range(0, Colors.Length)];
    }
}
