using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    [SerializeField] private bool drawGizmos = true;
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

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        int latitudeLines = 18; 
        int longitudeLines = 36;
        float radius = 1f;      

        Gizmos.color = Color.green;

        for (int i = 1; i < latitudeLines; i++)
        {
            float lat = Mathf.PI * i / latitudeLines;
            float y = Mathf.Cos(lat);
            float r = Mathf.Sin(lat);

            Vector3 prevPoint = Vector3.zero;
            for (int j = 0; j <= longitudeLines; j++)
            {
                float lon = 2 * Mathf.PI * j / longitudeLines;
                float x = Mathf.Cos(lon) * r;
                float z = Mathf.Sin(lon) * r;

                Vector3 point = new Vector3(x, y, z).normalized * radius;
                point = transform.TransformPoint(point);

                if (j > 0)
                    Gizmos.DrawLine(prevPoint, point);

                prevPoint = point;
            }
        }

        for (int i = 0; i < longitudeLines; i++)
        {
            float lon = 2 * Mathf.PI * i / longitudeLines;
            Vector3 prevPoint = Vector3.zero;

            for (int j = 0; j <= latitudeLines; j++)
            {
                float lat = Mathf.PI * j / latitudeLines;
                float y = Mathf.Cos(lat);
                float r = Mathf.Sin(lat);
                float x = Mathf.Cos(lon) * r;
                float z = Mathf.Sin(lon) * r;

                Vector3 point = new Vector3(x, y, z).normalized * radius;
                point = transform.TransformPoint(point);

                if (j > 0)
                    Gizmos.DrawLine(prevPoint, point);

                prevPoint = point;
            }
        }
    }
}
