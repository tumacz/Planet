using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Province
{
    public string Name;
    public Color Color;
    public int ID;
    public string Owner;

    public Vector2 UVCenter; // province center from UV
    public List<Vector2> UVOffsets = new(); // points around the province

    [System.NonSerialized] public Vector3? CachedWorldCenter = null;
    [System.NonSerialized] public Vector3? CachedNormal = null;
}

[CreateAssetMenu(fileName = "ProvinceDatabase", menuName = "World/Province Database")]
public class ProvinceDatabase : ScriptableObject
{
    public List<Province> Provinces = new List<Province>();

    private Dictionary<int, Province> _lookup;

    private void OnEnable()
    {
        _lookup = new Dictionary<int, Province>();
        foreach (var p in Provinces)
        {
            if (!_lookup.ContainsKey(p.ID))
                _lookup.Add(p.ID, p);
        }
    }

    public Province GetProvinceByColor(Color color)
    {
        int id = (Mathf.RoundToInt(color.r * 255) << 16) |
                 (Mathf.RoundToInt(color.g * 255) << 8) |
                 Mathf.RoundToInt(color.b * 255);

        _lookup ??= new Dictionary<int, Province>();
        _lookup.TryGetValue(id, out var province);
        return province;
    }
}