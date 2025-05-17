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

	public FrameFragment(Mesh mesh, int resolution, Vector3 localUp, Vector3 sphereCenter, float sphereRadius)
	{
		_mesh = mesh;
		_resolution = resolution;
		_localUp = localUp;
		_sphereCenter = sphereCenter;
		_sphereRadius = sphereRadius;

		_axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
		_axisB = Vector3.Cross(_localUp, _axisA);
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
				Vector3 direction = (worldPoint - _sphereCenter).normalized;
				Vector3 spherified = _sphereCenter + direction * _sphereRadius;
				vertices[i] = transform.InverseTransformPoint(spherified);

				uvs[i] = percent;

				if (x < _resolution - 1 && y < _resolution - 1)
				{
					triangles[triIndex] = i;
					triangles[triIndex + 1] = i + _resolution + 1;
					triangles[triIndex + 2] = i + _resolution;

					triangles[triIndex + 3] = i;
					triangles[triIndex + 4] = i + 1;
					triangles[triIndex + 5] = i + _resolution + 1;

					triIndex += 6;
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