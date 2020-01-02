using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MaterialFloorTagPair
{
    public string FloorTag;
    public Material Material;
}

public class FloorMaterialTags : MonoBehaviour
{
    public bool Uniform = true;
    public string Material = "Fallback";

    public MaterialFloorTagPair[] FloorTagPairs;
    Dictionary<Material, string> FloorTags;
    // Start is called before the first frame update
    void Awake()
    {
        FloorTags = new Dictionary<Material, string>();
        if (FloorTagPairs.Length > 0)
        {
            for (int i = 0; i < FloorTagPairs.Length; i++)
            {
                FloorTags.Add(FloorTagPairs[i].Material, FloorTagPairs[i].FloorTag);
            }
        }
    }
    public string ReturnMaterial(Material mat = null)
    {
        if (Uniform || mat == null)
        {
            return Material;
        }
        else 
        {
            for (int i = 0; i < FloorTagPairs.Length; i++)
            {
                MaterialFloorTagPair floorTagPair = FloorTagPairs[i];
                if (floorTagPair.Material != null && (floorTagPair.Material.name == mat.name || floorTagPair.Material.name + " (Instance)" == mat.name))
                {
                    return floorTagPair.FloorTag;

                }
            }
            return "Fallback";
        }
    }
}
