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
	private float _uniformScale;

	public FrameFragment(
		Mesh mesh,
		int resolution,
		Vector3 localUp,
		Vector3 sphereCenter,
		float sphereRadius,
		Material material,
		float heightScale,
		float uniformScale)
	{
		_mesh = mesh;
		_resolution = resolution;
		_localUp = localUp;
		_sphereCenter = sphereCenter;
		_sphereRadius = sphereRadius;
		_material = material;
		_heightScale = heightScale;
		_uniformScale = uniformScale;

		_axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
		_axisB = Vector3.Cross(_localUp, _axisA);

		if (_material != null && _material.HasProperty("_HeightMap"))
		{
			_heightMap = _material.GetTexture("_HeightMap") as Texture2D;
		}
	}

	public void ConstructMesh(Transform transform, PlanetMeta planetMeta = null, ProvinceDatabase provinceDB = null, int layer = -1)
	{
		Vector3[] vertices = new Vector3[_resolution * _resolution];
		Vector2[] uvs = new Vector2[vertices.Length];
		int[] triangles = new int[(_resolution - 1) * (_resolution - 1) * 6];
		int triIndex = 0;

		Vector3 scaleVec = Vector3.one * _uniformScale;
		Vector2 minUV = new Vector2(1f, 1f);
		Vector2 maxUV = new Vector2(0f, 0f);

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

				float u = 0.5f + Mathf.Atan2(dir.z, dir.x) / (2f * Mathf.PI);
				float v = 0.5f + Mathf.Asin(dir.y) / Mathf.PI;

				minUV = Vector2.Min(minUV, new Vector2(u, v));
				maxUV = Vector2.Max(maxUV, new Vector2(u, v));

				float height = SampleHeightAtUV(u, v);
				Vector3 displaced = _sphereCenter + dir * (_sphereRadius + height);
				Vector3 scaledDisplaced = Vector3.Scale(displaced, scaleVec);

				vertices[i] = transform.InverseTransformPoint(scaledDisplaced);
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
		Vector3 scaledCenter = Vector3.Scale(_sphereCenter, scaleVec);
		for (int i = 0; i < normals.Length; i++)
		{
			normals[i] = (vertices[i] - transform.InverseTransformPoint(scaledCenter)).normalized;
		}
		_mesh.normals = normals;

		if (layer >= 0)
			transform.gameObject.layer = layer;

		// 🌍 Rejestracja danych prowincji do meta
		if (planetMeta != null && provinceDB != null && planetMeta.DataAsset != null)
		{
			foreach (var province in provinceDB.GetAllProvinces())
			{
				Vector2 uv = province.UVCenter;
				if (uv.x < minUV.x || uv.x > maxUV.x || uv.y < minUV.y || uv.y > maxUV.y)
					continue;

				float longitude = (uv.x - 0.5f) * 2f * Mathf.PI;
				float latitude = (uv.y - 0.5f) * Mathf.PI;

				Vector3 dir = new Vector3(
					Mathf.Cos(latitude) * Mathf.Sin(longitude),
					Mathf.Sin(latitude),
					Mathf.Cos(latitude) * Mathf.Cos(longitude)
				).normalized;

				float height = SampleHeightAtUV(uv.x, uv.y);
				Vector3 displaced = _sphereCenter + dir * (_sphereRadius + height);
				Vector3 world = Vector3.Scale(displaced, scaleVec);
				Vector3 normal = (world - _sphereCenter).normalized;

				planetMeta.Register(province.ID, world, normal);
				province.CacheWorldData(world, normal);
			}
		}
	}

	private float SampleHeightAtUV(float u, float v)
	{
		if (_heightMap == null) return 0f;

		Vector2 adjustedUV = new Vector2(u, v);
		if (_material.HasProperty("_HeightMap_ST"))
		{
			Vector4 st = _material.GetVector("_HeightMap_ST");
			adjustedUV.x = u * st.x + st.z;
			adjustedUV.y = v * st.y + st.w;
		}

		Color pixel = _heightMap.GetPixelBilinear(adjustedUV.x, adjustedUV.y);
		return pixel.r * _heightScale;
	}
}