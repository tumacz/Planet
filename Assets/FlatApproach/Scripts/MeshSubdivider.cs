using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class MeshSubdivider : MonoBehaviour
{
	[Range(2, 255)] public int subdivisions = 20;
	[Range(-1, 1)] public float heightScale = 1.0f;
	public Texture2D heightMap;
	[HideInInspector] public Rect uvRect = new Rect(0, 0, 1, 1);
	public bool useXZPlane = false;

	public void Regenerate()
	{
		SubdivideMesh();
	}

	private void OnValidate()
	{
		SubdivideMesh();
	}

	private void SubdivideMesh()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null || heightMap == null)
		{
			return;
		}

		int width = subdivisions + 1;
		int height = subdivisions + 1;

		Vector3[] vertices = new Vector3[width * height];
		Vector2[] uvs = new Vector2[width * height];
		int[] triangles = new int[subdivisions * subdivisions * 6];

		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				int i = x + y * width;
				float u = (float)x / (width - 1);
				float v = (float)y / (height - 1);

				// Skaluje UV do fragmentu heightmapy (globalne UV dla BaseMap)
				float realU = uvRect.x + u * uvRect.width;
				float realV = uvRect.y + v * uvRect.height;

				Color pixel = heightMap.GetPixelBilinear(realU, realV);
				float heightValue = pixel.r * heightScale;

				if (useXZPlane)
				{
					vertices[i] = new Vector3(u - 0.5f, heightValue, v - 0.5f);
				}
				else
				{
					vertices[i] = new Vector3(u - 0.5f, v - 0.5f, heightValue);
				}

				uvs[i] = new Vector2(realU, realV);

			}
		}

		int t = 0;
		for (int y = 0; y < subdivisions; y++)
		{
			for (int x = 0; x < subdivisions; x++)
			{
				int i = x + y * width;

				triangles[t++] = i;
				triangles[t++] = i + width;
				triangles[t++] = i + 1;

				triangles[t++] = i + 1;
				triangles[t++] = i + width;
				triangles[t++] = i + width + 1;
			}
		}

		Mesh subdividedMesh = new Mesh();
		subdividedMesh.name = "SubdividedWithHeight";
		subdividedMesh.vertices = vertices;
		subdividedMesh.uv = uvs;
		subdividedMesh.triangles = triangles;
		subdividedMesh.RecalculateNormals();

		meshFilter.sharedMesh = subdividedMesh;
	}
}