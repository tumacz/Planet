using UnityEngine;

public static class MarkerSpawner
{
	private static GameObject _defaultMarkerPrefab;

	/// <summary>
	/// Ustaw prefab do spawnowania markerów.
	/// </summary>
	public static void SetMarkerPrefab(GameObject prefab)
	{
		_defaultMarkerPrefab = prefab;
	}

	/// <summary>
	/// Tworzy marker w danym miejscu z okreœlon¹ skal¹ i kolorem.
	/// </summary>
	public static GameObject SpawnMarker(string name, Vector3 position, Quaternion rotation, float scale, Color color, Transform parent = null)
	{
		GameObject marker;

		if (_defaultMarkerPrefab != null)
		{
			marker = Object.Instantiate(_defaultMarkerPrefab, position, rotation);
		}
		else
		{
			marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
			marker.transform.position = position;
			marker.transform.rotation = rotation;
		}

		marker.name = $"Ziemniak_{name}";
		marker.transform.localScale = Vector3.one * scale;

		if (color.a > 0f && marker.TryGetComponent(out Renderer renderer))
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				renderer.sharedMaterial.color = color;
			else
#endif
				renderer.material.color = color;
		}

		if (parent != null)
			marker.transform.SetParent(parent, true);

#if UNITY_EDITOR
		if (!Application.isPlaying)
			UnityEditor.Undo.RegisterCreatedObjectUndo(marker, "Spawn Marker");
#endif

		return marker;
	}
}
