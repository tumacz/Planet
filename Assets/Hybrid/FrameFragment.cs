using UnityEngine;

public class FrameFragment
{
	private Mesh _mesh;
	private int _resolution;
	private Vector3 _localUp;
	private Vector3 _axisA;
	private Vector3 _axisB;
	private Vector3 _sphereCenter;
	private float _sphereRadius;
	private float _heightScale;
	private Material _material;
	private Texture2D _heightMap;

	public FrameFragment(Mesh mesh, int resolution, Vector3 localUp, Vector3 sphereCenter, float sphereRadius, Material material, float heightScale)
	{
		_mesh = mesh;
		_resolution = resolution;
		_localUp = localUp;
		_sphereCenter = sphereCenter;
		_sphereRadius = sphereRadius;
		_material = material;
		_heightScale = heightScale;

		_axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
		_axisB = Vector3.Cross(_localUp, _axisA);

		if (_material != null && _material.HasProperty("_HeightMap"))
		{
			_heightMap = _material.GetTexture("_HeightMap") as Texture2D;
		}
	}

	public void ConstructMesh(Transform transform)
	{
		Vector3[] vertices = new Vector3[_resolution * _resolution];
		Vector2[] uvs = new Vector2[vertices.Length];
		int[] triangles = new int[(_resolution - 1) * (_resolution - 1) * 6];
		int triIndex = 0;

		for (int y = 0; y < _resolution; y++)
		{
			for (int x = 0; x < _resolution; x++)
			{
				int i = x + y * _resolution;
				Vector2 percent = new Vector2(x, y) / (_resolution - 1);

				Vector3 pointOnFace = _localUp +
					(percent.x - 0.5f) * 2f * _axisA +
					(percent.y - 0.5f) * 2f * _axisB;

				Vector3 worldPoint = transform.TransformPoint(pointOnFace);
				Vector3 dir = (worldPoint - _sphereCenter).normalized;
				Vector3 pointOnSphere = _sphereCenter + dir * _sphereRadius;

				float u = 0.5f + Mathf.Atan2(dir.z, dir.x) / (2f * Mathf.PI);
				//float v = 0.5f - Mathf.Asin(dir.y) / Mathf.PI;
				float v = 0.5f + Mathf.Asin(dir.y) / Mathf.PI; // <- Flipped V


				float height = 0f;
				if (_heightMap != null)
				{
					float uAdj = u, vAdj = v;
					if (_material.HasProperty("_HeightMap_ST"))
					{
						Vector4 st = _material.GetVector("_HeightMap_ST");
						uAdj = u * st.x + st.z;
						vAdj = v * st.y + st.w;
					}

					Color pixel = _heightMap.GetPixelBilinear(uAdj, vAdj);
					height = pixel.r * _heightScale;
				}

				Vector3 displaced = _sphereCenter + dir * (_sphereRadius + height);
				vertices[i] = transform.InverseTransformPoint(displaced);
				uvs[i] = new Vector2(u, v);

				if (x < _resolution - 1 && y < _resolution - 1)
				{
					triangles[triIndex++] = i;
					triangles[triIndex++] = i + _resolution + 1;
					triangles[triIndex++] = i + _resolution;

					triangles[triIndex++] = i;
					triangles[triIndex++] = i + 1;
					triangles[triIndex++] = i + _resolution + 1;
				}
			}
		}

		_mesh.Clear();
		_mesh.vertices = vertices;
		_mesh.triangles = triangles;
		_mesh.uv = uvs;

		Vector3[] normals = new Vector3[vertices.Length];
		for (int i = 0; i < normals.Length; i++)
		{
			normals[i] = (vertices[i] - transform.InverseTransformPoint(_sphereCenter)).normalized;
		}
		_mesh.normals = normals;
	}
}