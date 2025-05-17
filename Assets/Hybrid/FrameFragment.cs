using UnityEngine;

public class FrameFragment : MonoBehaviour
{
	Mesh _mesh;
	private int _obtainedFrameFragmentResolution;
	Vector3 _localup;
	Vector3 _axisA;
	Vector3 _axisB;

	public FrameFragment(Mesh mesh, int resolution, Vector3 localup)
	{
		this._mesh = mesh;
		this._obtainedFrameFragmentResolution = resolution;
		this._localup = localup;

		_axisA = new Vector3(localup.y, localup.z, localup.x);
		_axisB = Vector3.Cross(localup, _axisA);
	}

	public void ConstructMesh()
	{
		Vector3[] vertices = new Vector3[_obtainedFrameFragmentResolution * _obtainedFrameFragmentResolution];
		Vector2[] uvs = new Vector2[vertices.Length];
		int[] triangles = new int[(_obtainedFrameFragmentResolution - 1) * (_obtainedFrameFragmentResolution - 1) * 6];
		int vertexIndex = 0;

		for (int y = 0; y < _obtainedFrameFragmentResolution; y++)
		{
			for (int x = 0; x < _obtainedFrameFragmentResolution; x++)
			{
				int i = x + y * _obtainedFrameFragmentResolution;
				Vector2 percent = new Vector2(x, y) / (_obtainedFrameFragmentResolution - 1);
				//Vector3 pointOnUnitCube = _localup + (percent.x - 0.5f) * 2 * _axisA + (percent.y - 0.5f) * 2 * _axisB;
				//Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
				//vertices[i] = pointOnUnitSphere;
				Vector3 pointOnCubeFace = _localup + (percent.x - 0.5f) * 2 * _axisA + (percent.y - 0.5f) * 2 * _axisB;
				vertices[i] = pointOnCubeFace;

				uvs[i] = percent;

				if (x < _obtainedFrameFragmentResolution - 1 && y < _obtainedFrameFragmentResolution - 1)
				{
					triangles[vertexIndex] = i;
					triangles[vertexIndex + 1] = i + _obtainedFrameFragmentResolution + 1;
					triangles[vertexIndex + 2] = i + _obtainedFrameFragmentResolution;

					triangles[vertexIndex + 3] = i;
					triangles[vertexIndex + 4] = i + 1;
					triangles[vertexIndex + 5] = i + _obtainedFrameFragmentResolution + 1;

					vertexIndex += 6;
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
			normals[i] = vertices[i].normalized;
		}
		_mesh.normals = normals;
	}
}