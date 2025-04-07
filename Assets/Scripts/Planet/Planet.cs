using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    [SerializeField] private Material material;
    [SerializeField, Range(2, 256)] private int resolution = 10;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    private void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        terrainFaces = new TerrainFace[6];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < meshFilters.Length; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("TerrainFace" + i);
                meshObject.transform.parent = transform;

                if (material != null)
                {
                    meshObject.AddComponent<MeshRenderer>().sharedMaterial = material;
                }
                else
                {
                    meshObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                }
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].mesh = new Mesh();
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    private void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }
}