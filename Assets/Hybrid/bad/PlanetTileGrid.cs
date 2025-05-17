using UnityEngine;

[ExecuteInEditMode]
public class PlanetTileGrid : MonoBehaviour
{
	[Header("Rendering")]
	[SerializeField] private Material tileMaterial;
	[SerializeField, Range(2, 256)] private int tileResolution = 2;

	[Header("Face Tiling")]
	[SerializeField, Range(1, 20)] private int tilesPerFace = 2;

	private void OnValidate()
	{
		Generate();
	}

	private void Generate()
	{
		ClearChildren();

		Vector3[] directions = {
				Vector3.up, Vector3.down, Vector3.left,
				Vector3.right, Vector3.forward, Vector3.back
			};

		for (int i = 0; i < 6; i++)
		{
			GameObject face = new GameObject("FaceTile_" + directions[i]);
			face.transform.parent = transform;
			face.transform.localPosition = Vector3.zero;

			var faceTile = face.AddComponent<TerrainFaceTile>();
			faceTile.Setup(directions[i], tileResolution, tilesPerFace, tileMaterial);
		}
	}

	private void ClearChildren()
	{
	#if UNITY_EDITOR
		while (transform.childCount > 0)
			DestroyImmediate(transform.GetChild(0).gameObject);
	#else
	        while (transform.childCount > 0)
	            Destroy(transform.GetChild(0).gameObject);
	#endif
	}
}

