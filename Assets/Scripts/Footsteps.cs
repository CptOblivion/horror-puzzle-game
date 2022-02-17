using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class FootstepMaterial
{
  public string FloorMaterial;
  public AudioClip[] StepSounds = new AudioClip[0];
}
public class Footsteps : MonoBehaviour
{
  public bool CheckSurface = true;
  public Transform FootLocation;
  public AudioClip[] StepSoundsFallback;
  public FootstepMaterial[] StepSounds_Container;
  Dictionary<string, AudioClip[]> StepSounds;

  public float PitchRand = .1f;

  private void Awake()
  {
    StepSounds = new Dictionary<string, AudioClip[]>();
    StepSounds.Add("Fallback", StepSoundsFallback);
    for (int i = 0; i < StepSounds_Container.Length; i++)
    {
      StepSounds.Add(StepSounds_Container[i].FloorMaterial, StepSounds_Container[i].StepSounds);
    }
  }
  public void Footstep()
  {
    string FloorMaterial = "Fallback";
    AudioClip ChosenSound;

    if (CheckSurface)
    {
      Vector3 CastOrigin = transform.position;
      if (FootLocation)
      {
        CastOrigin = FootLocation.position;
      }

      float CastDistance = 1;

      Ray ray = new Ray(CastOrigin, -transform.up);
      if (Physics.Raycast(ray, out RaycastHit hit, CastDistance))
      {
        FloorMaterialTags floorMaterialTags = hit.collider.GetComponent<FloorMaterialTags>();
        Renderer renderer = hit.collider.GetComponent<Renderer>();
        if (floorMaterialTags && floorMaterialTags.Uniform)
        {
          FloorMaterial = floorMaterialTags.ReturnMaterial();
        }
        else if (floorMaterialTags && renderer)
        {
          if (renderer.materials.Length == 1)
          {
            FloorMaterial = floorMaterialTags.ReturnMaterial(renderer.materials[0]);
          }
          else
          {
            //we gotta get the material of the triangle that the ray hit
            int TriangleIndex = hit.triangleIndex;
            MeshFilter meshFilter = hit.collider.gameObject.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;
            bool FoundMaterial = false;
            int[] HitTriangle = new int[] { mesh.triangles[TriangleIndex * 3], mesh.triangles[TriangleIndex * 3 + 1], mesh.triangles[TriangleIndex * 3 + 2] };

            //LogIntList(HitTriangle);

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
              int[] SubMeshTris = mesh.GetTriangles(i);
              for (int j = 0; j < SubMeshTris.Length; j += 3)
              {
                int[] TestTri = new int[] { SubMeshTris[j], SubMeshTris[j + 1], SubMeshTris[j + 2] };
                //LogIntList(TestTri);
                if (TestTri[0] == HitTriangle[0] && TestTri[1] == HitTriangle[1] && TestTri[2] == HitTriangle[2])
                {
                  FloorMaterial = floorMaterialTags.ReturnMaterial(renderer.materials[i]);
                  FoundMaterial = true;
                  break;
                }
              }
            }

            if (!FoundMaterial)
            {
              Debug.Log("No material found!");
            }
          }
        }
      }
    }
    if (!StepSounds.ContainsKey(FloorMaterial) || StepSounds[FloorMaterial].Length == 0)
    {
      FloorMaterial = "Fallback";
    }

    AudioClip[] FloorMaterialSounds = StepSounds[FloorMaterial];
    ChosenSound = FloorMaterialSounds[Random.Range(0, FloorMaterialSounds.Length)];
    if (ChosenSound != null) PlaySFX.Play(ChosenSound, Random.Range(1 - PitchRand, 1 + PitchRand), transform.position);
  }

  public static void LogIntList(int[] intList)
  {
    string printString = "";
    for (int i = 0; i < intList.Length; i++)
    {
      printString += intList[i] + ", ";
    }
    Debug.Log(printString);
  }
}
