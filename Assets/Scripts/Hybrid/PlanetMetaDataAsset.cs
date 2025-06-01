using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetMetaDataAsset", menuName = "World/Planet Meta Data")]
public class PlanetMetaDataAsset : ScriptableObject
{
	[System.Serializable]
	public class ProvinceWorldData
	{
		public int ProvinceID;
		public Vector3 WorldPosition;
		public Vector3 Normal;
	}

	[SerializeField] private List<ProvinceWorldData> _dataList = new();
	private Dictionary<int, ProvinceWorldData> _lookup;

	public void Clear()
	{
		_dataList.Clear();
		_lookup = null;
	}

	public void Register(int provinceID, Vector3 worldPos, Vector3 normal)
	{
		_lookup ??= BuildLookup();

		if (_lookup.TryGetValue(provinceID, out var existing))
		{
			if (existing.WorldPosition == worldPos && existing.Normal == normal)
				return;

			existing.WorldPosition = worldPos;
			existing.Normal = normal;
		}
		else
		{
			var data = new ProvinceWorldData
			{
				ProvinceID = provinceID,
				WorldPosition = worldPos,
				Normal = normal
			};
			_dataList.Add(data);
			_lookup[provinceID] = data;
		}
	}

	public bool TryGetPosition(int provinceID, out Vector3 pos, out Vector3 normal)
	{
		_lookup ??= BuildLookup();
		if (_lookup.TryGetValue(provinceID, out var d))
		{
			pos = d.WorldPosition;
			normal = d.Normal;
			return true;
		}

		pos = Vector3.zero;
		normal = Vector3.up;
		return false;
	}

	private Dictionary<int, ProvinceWorldData> BuildLookup()
	{
		var dict = new Dictionary<int, ProvinceWorldData>();
		foreach (var item in _dataList)
		{
			if (!dict.ContainsKey(item.ProvinceID))
				dict.Add(item.ProvinceID, item);
		}
		return dict;
	}

	public List<ProvinceWorldData> GetAll()
	{
		return _dataList;
	}
}