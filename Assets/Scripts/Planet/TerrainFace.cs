using UnityEngine;

public class TerrainFace
{
    Mesh mesh;
    private int resolution;
    Vector3 localup;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(Mesh mesh, int resolution, Vector3 localup)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localup = localup;

        axisA = new Vector3(localup.y, localup.z, localup.x);
        axisB = Vector3.Cross(localup, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int vertexIndex = 0;

        for (int y = 0; y < resolution; y++)
        { 
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localup + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = pointOnUnitSphere;

                if (x < resolution - 1 && y < resolution - 1)
                {
                    triangles[vertexIndex] = i;
                    triangles[vertexIndex + 1] = i + resolution + 1;
                    triangles[vertexIndex + 2] = i + resolution;

                    triangles[vertexIndex + 3] = i;
                    triangles[vertexIndex + 4] = i + 1;
                    triangles[vertexIndex + 5] = i + resolution + 1;

                    vertexIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = vertices[i].normalized;
        }
        mesh.normals = normals;
    }
}
