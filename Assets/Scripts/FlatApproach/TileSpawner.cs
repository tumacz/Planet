//using UnityEngine;
//using UnityEditor;

//[ExecuteAlways]
//public class TileSpawner : MonoBehaviour
//{
//	public GameObject prefab;
//	public Texture2D heightMap;
//	public Texture2D normalMap;
//	public Texture2D baseMap;
//	[Range(1, 40)] public int rows = 10;
//	[Range(1, 80)] public int columns = 10;
//	public float spacing = 1.0f;

//	[Range(2, 255)] public int globalSubdivisions = 20;
//	[Range(-1, 1)] public float globalHeightScale = 1.0f;

//	private bool needsClear = false;

//	private void OnValidate()
//	{
//		if (prefab != null)
//		{
//			needsClear = true;
//			EditorApplication.update -= PerformClearAndSpawn;
//			EditorApplication.update += PerformClearAndSpawn;
//		}
//	}

//	private void OnDestroy()
//	{
//		EditorApplication.update -= PerformClearAndSpawn;
//	}

//	//private void PerformClearAndSpawn()
//	//{
//	//	EditorApplication.update -= PerformClearAndSpawn;

//	//	if (!needsClear) return;
//	//	needsClear = false;

//	//	ClearTiles();
//	//	SpawnTiles();
//	//}
//	private void PerformClearAndSpawn()
//	{
//		// Jeœli obiekt zosta³ zniszczony, natychmiast odpinamy i wychodzimy
//		if (this == null || transform == null)
//		{
//			EditorApplication.update -= PerformClearAndSpawn;
//			return;
//		}

//		EditorApplication.update -= PerformClearAndSpawn;

//		if (!needsClear) return;
//		needsClear = false;

//		ClearTiles();
//		SpawnTiles();
//	}


//	private void ClearTiles()
//	{
//		for (int i = transform.childCount - 1; i >= 0; i--)
//		{
//			var child = transform.GetChild(i).gameObject;
//#if UNITY_EDITOR
//			DestroyImmediate(child);
//#else
//            Destroy(child);
//#endif
//		}
//	}

//	private void SpawnTiles()
//	{
//		if (heightMap == null)
//		{
//			Debug.LogWarning("No hight map!");
//			return;
//		}

//		for (int y = 0; y < rows; y++)
//		{
//			for (int x = 0; x < columns; x++)
//			{
//				GameObject tile = Instantiate(prefab, transform);
//				tile.transform.localPosition = new Vector3(x * spacing, y * spacing, 0);

//				float uStart = (float)x / columns;
//				float vStart = (float)y / rows;
//				float uSize = 1.0f/ columns;
//				float vSize = 1.0f/ rows;
//				Rect uvRect = new Rect(uStart, vStart, uSize, vSize);

//				var chunk = tile.GetComponent<TileChunk>();
//				if (chunk != null)
//				{
//					chunk.Setup(heightMap, uvRect);
//				}

//				var subdivider = tile.GetComponent<MeshSubdivider>();
//				if (subdivider != null)
//				{
//					subdivider.subdivisions = globalSubdivisions;
//					subdivider.heightScale = globalHeightScale;
//					subdivider.Regenerate();
//				}

//				// <<< SET MATERIAL >>>
//				var renderer = tile.GetComponent<MeshRenderer>();
//				if (renderer != null && renderer.sharedMaterial != null)
//				{
//					Material mat = renderer.sharedMaterial;

//					if (mat.HasProperty("_HeightMap"))
//						mat.SetTexture("_HeightMap", heightMap);

//					if (mat.HasProperty("_NormalMap"))
//						mat.SetTexture("_NormalMap", normalMap);

//					if (mat.HasProperty("_BaseMap"))
//						mat.SetTexture("_BaseMap", baseMap);

//					if (mat.HasProperty("_UVRect"))
//						mat.SetVector("_UVRect", new Vector4(uStart, vStart, uSize, vSize));
//				}
//			}
//		}
//	}
//}