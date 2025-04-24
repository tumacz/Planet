using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    [Header("Rendering")]
    [SerializeField] private Material material;
    [SerializeField, Range(2, 256)] private int resolution = 10;

    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos = true;
    [SerializeField] private bool showUVLabels = false;
    [SerializeField, Range(1, 36)] private int uvLabelStep = 6;
    [SerializeField] private Vector3 uvLabelOffset = new Vector3(0.05f, 0.05f, 0.05f);
    [SerializeField] private Color gizmoColor = Color.cyan;


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

                meshObject.AddComponent<MeshRenderer>().sharedMaterial = material ?? new Material(Shader.Find("Standard"));
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

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        GizmoUV.DrawSphereGrid(
            transform,
            showUVLabels,
            uvLabelStep,
            uvLabelOffset,
            latitudeLines: 18,
            longitudeLines: 36,
            radius: 1f,
            gizCol: gizmoColor
        );
    }

}
