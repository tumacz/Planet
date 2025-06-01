using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Province
{
	public string Name;
	public Color Color;
	public int ID;
	public string Owner;
	public Population Population;

	public Vector2 UVCenter;
	public List<Vector2> UVOffsets = new();

	[System.NonSerialized] public Vector3? CachedWorldCenter = null;
	[System.NonSerialized] public Vector3? CachedNormal = null;

	public void CacheWorldData(Vector3 worldPos, Vector3 normal)
	{
		CachedWorldCenter = worldPos;
		CachedNormal = normal;
	}
}