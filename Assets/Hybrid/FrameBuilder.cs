using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class FrameBuilder : MonoBehaviour
{
	[Header("Rendering")]
	[SerializeField] private Material _material;
	[SerializeField, Range(2, 255)] private int _frameFragmentResolution = 2;
	[SerializeField, Range(1, 6)] private int _frameFaceResolution = 1;
	[SerializeField, Range(-1f, 1f)] private float heightScale = 1.0f;

	private Vector3 _builderPoint;
	private bool _needsRebuild = false;

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			_needsRebuild = true;
			EditorApplication.update -= PerformUpdate;
			EditorApplication.update += PerformUpdate;
		}
	}

	private void OnDestroy()
	{
		EditorApplication.update -= PerformUpdate;
	}

	private void PerformUpdate()
	{
		EditorApplication.update -= PerformUpdate;

		if (!_needsRebuild) return;
		_needsRebuild = false;

		Clear();
		Generate();
	}

	private void Clear()
	{
#if UNITY_EDITOR
		for (int i = transform.childCount - 1; i >= 0; i--)
			DestroyImmediate(transform.GetChild(i).gameObject);
#else
		foreach (Transform child in transform)
			Destroy(child.gameObject);
#endif
	}

	private void Generate()
	{
		_builderPoint = transform.position;
		float fragmentSize = 2f;
		int faceRes = _frameFaceResolution;
		float sphereRadius = faceRes * fragmentSize * 0.5f;

		for (int f = 0; f < 6; f++)
		{
			string faceName = "";
			Vector3 localUp = Vector3.forward;
			Vector3 axisA = Vector3.right;
			Vector3 axisB = Vector3.up;
			Vector3 faceRootPos = Vector3.zero;

			switch (f)
			{
				case 0: faceName = "Back"; localUp = Vector3.back; axisA = Vector3.right; axisB = Vector3.up; faceRootPos = new Vector3(0, 0, -sphereRadius); break;
				case 1: faceName = "Right"; localUp = Vector3.right; axisA = Vector3.forward; axisB = Vector3.up; faceRootPos = new Vector3(sphereRadius, 0, 0); break;
				case 2: faceName = "Up"; localUp = Vector3.up; axisA = Vector3.right; axisB = Vector3.forward; faceRootPos = new Vector3(0, sphereRadius, 0); break;
				case 3: faceName = "Front"; localUp = Vector3.forward; axisA = Vector3.right; axisB = Vector3.up; faceRootPos = new Vector3(0, 0, sphereRadius); break;
				case 4: faceName = "Left"; localUp = Vector3.left; axisA = Vector3.forward; axisB = Vector3.up; faceRootPos = new Vector3(-sphereRadius, 0, 0); break;
				case 5: faceName = "Down"; localUp = Vector3.down; axisA = Vector3.right; axisB = Vector3.forward; faceRootPos = new Vector3(0, -sphereRadius, 0); break;
			}

			GameObject faceRoot = new GameObject("Face_" + faceName);
			faceRoot.transform.parent = transform;
			faceRoot.transform.localPosition = faceRootPos - localUp * 1f;
			faceRoot.transform.localRotation = Quaternion.identity;

			float start = -faceRes * fragmentSize * 0.5f + fragmentSize * 0.5f;

			for (int y = 0; y < faceRes; y++)
			{
				for (int x = 0; x < faceRes; x++)
				{
					float offsetX = start + x * fragmentSize;
					float offsetY = start + y * fragmentSize;
					Vector3 localOffset = offsetX * axisA + offsetY * axisB;

					GameObject tile = new GameObject($"Fragment_{x}_{y}_{faceName}");
					tile.transform.parent = faceRoot.transform;
					tile.transform.localPosition = localOffset;
					tile.transform.localRotation = Quaternion.identity;

					Mesh mesh = new Mesh();
					var meshFilter = tile.AddComponent<MeshFilter>();
					var meshRenderer = tile.AddComponent<MeshRenderer>();
					meshFilter.sharedMesh = mesh;
					meshRenderer.sharedMaterial = _material ?? new Material(Shader.Find("Standard"));

					var fragment = new FrameFragment(mesh, _frameFragmentResolution, localUp, _builderPoint, sphereRadius, _material, heightScale);
					fragment.ConstructMesh(tile.transform);
				}
			}
		}
	}
}