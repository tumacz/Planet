using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class FrameBuilder : MonoBehaviour
{
	[Header("Rendering")]
	[SerializeField] private Material _material;
	[SerializeField, Range(2, 255)] private int _frameFragmentResolution = 2;
	[SerializeField, Range(1, 40)] private int _frameFaceResolution = 1;
	[SerializeField, Range(-1f, 1f)] private float heightScale = 1.0f;

	[Header("Mesh Scaling")]
	[SerializeField, Min(0.01f)] private float uniformMeshScale = 1f;

	[Header("Layer Settings")]
	[SerializeField] private string globeLayerName = "Globe";

	[Header("Meta References")]
	[SerializeField] private PlanetMeta planetMeta;
	[SerializeField] private ProvinceDatabase provinceDatabase;

	private Vector3 _builderPoint;
	private bool _needsRebuild = false;

	public int FrameFaceResolution => _frameFaceResolution;

#if UNITY_EDITOR
	private bool IsSceneObject =>
		PrefabUtility.GetPrefabAssetType(gameObject) == PrefabAssetType.NotAPrefab ||
		PrefabUtility.IsPartOfPrefabInstance(gameObject);
#endif

	private void OnValidate()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying && IsSceneObject)
		{
			_needsRebuild = true;
			EditorApplication.delayCall -= PerformUpdate;
			EditorApplication.delayCall += PerformUpdate;
		}
#endif
	}

	private void OnDestroy()
	{
#if UNITY_EDITOR
		EditorApplication.delayCall -= PerformUpdate;
#endif
	}

	private void PerformUpdate()
	{
#if UNITY_EDITOR
		EditorApplication.delayCall -= PerformUpdate;

		// 🛑 Obiekt mógł zostać zniszczony – przerwij, jeśli już nie istnieje
		if (this == null)
			return;

		if (!_needsRebuild) return;
		_needsRebuild = false;

		Clear();
		Generate();
#endif
	}

	private void Clear()
	{
#if UNITY_EDITOR
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			var child = transform.GetChild(i).gameObject;
			DestroyImmediate(child, true);
		}
#endif
	}

	private void Generate()
	{
		_builderPoint = transform.position;
		float fragmentSize = 2f;
		int faceRes = _frameFaceResolution;
		float sphereRadius = faceRes * fragmentSize * 0.5f;

		int globeLayer = LayerMask.NameToLayer(globeLayerName);
		if (globeLayer == -1)
			Debug.LogWarning($"Layer '{globeLayerName}' not found. Please create it in Tags & Layers.");

		if (planetMeta != null)
		{
			planetMeta.Clear();
			planetMeta.SphereRadius = sphereRadius;
		}

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
			faceRoot.transform.SetParent(transform, false);
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
					tile.transform.SetParent(faceRoot.transform, false);
					tile.transform.localPosition = localOffset;
					tile.transform.localRotation = Quaternion.identity;

					if (globeLayer != -1)
						tile.layer = globeLayer;

					Mesh mesh = new Mesh();
					var meshFilter = tile.AddComponent<MeshFilter>();
					var meshRenderer = tile.AddComponent<MeshRenderer>();
					meshFilter.sharedMesh = mesh;
					meshRenderer.sharedMaterial = _material ?? new Material(Shader.Find("Standard"));

					var meshCollider = tile.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = mesh;

					var fragment = new FrameFragment(mesh, _frameFragmentResolution, localUp, _builderPoint, sphereRadius, _material, heightScale, uniformMeshScale);
					fragment.ConstructMesh(tile.transform, planetMeta, provinceDatabase);
				}
			}
		}
	}
}