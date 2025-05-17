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

    public Vector2 UVCenter; // province center
    public List<Vector2> UVOffsets = new(); // specific points in the province

    [System.NonSerialized] public Vector3? CachedWorldCenter = null;
    [System.NonSerialized] public Vector3? CachedNormal = null;
}