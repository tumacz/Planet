using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshTile : MonoBehaviour
{
	public void Build(Vector3 localUp, Vector3 axisA, Vector3 axisB, int res, Rect uvRect, Material mat)
	{
		MeshFilter filter = GetComponent<MeshFilter>();
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		renderer.sharedMaterial = mat;

		int vertexCount = res * res;
		Vector3[] vertices = new Vector3[vertexCount];
		Vector2[] uvs = new Vector2[vertexCount];
		int[] triangles = new int[(res - 1) * (res - 1) * 6];
		int triIndex = 0;

		for (int y = 0; y < res; y++)
		{
			for (int x = 0; x < res; x++)
			{
				int i = x + y * res;
				Vector2 percent = new Vector2(x, y) / (res - 1);
				Vector3 pointOnCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
				Vector3 pointOnSphere = pointOnCube.normalized;
				vertices[i] = pointOnSphere;

				// Skalowane UV na atlas
				uvs[i] = new Vector2(
					uvRect.x + uvRect.width * percent.x,
					uvRect.y + uvRect.height * percent.y
				);

				if (x < res - 1 && y < res - 1)
				{
					int a = i;
					int b = i + res;
					int c = i + 1;
					int d = i + res + 1;

					triangles[triIndex++] = a;
					triangles[triIndex++] = d;
					triangles[triIndex++] = b;

					triangles[triIndex++] = a;
					triangles[triIndex++] = c;
					triangles[triIndex++] = d;
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals();

		filter.sharedMesh = mesh;
	}
}

