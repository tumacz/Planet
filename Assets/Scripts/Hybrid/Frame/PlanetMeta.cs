//using UnityEngine;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//[ExecuteInEditMode]
//[DisallowMultipleComponent]
//public class PlanetMeta : MonoBehaviour
//{
//	[Header("Geometry Info")]
//	public float SphereRadius = 1f;

//	[Header("Data")]
//	public PlanetMetaDataAsset DataAsset;

//	public void Clear()
//	{
//		DataAsset?.Clear();
//	}

//	public void Register(int provinceID, Vector3 pos, Vector3 normal)
//	{
//		DataAsset?.Register(provinceID, pos, normal);
//	}

//	//public bool TryGetPosition(int provinceID, out Vector3 pos, out Vector3 normal)
//	//{
//	//	if (DataAsset != null)
//	//		return DataAsset.TryGetPosition(provinceID, out pos, out normal);

//	//	pos = Vector3.zero;
//	//	normal = Vector3.up;
//	//	return false;
//	//}

//	[ContextMenu("DEBUG: Spawn Markers for All Provinces")]
//	public void DebugAllProvinces()
//	{
//#if UNITY_EDITOR
//		if (!Application.isPlaying && DataAsset != null)
//		{
//			var allData = DataAsset.GetAll();
//			foreach (var data in allData)
//			{
//				Vector3 pos = data.WorldPosition;
//				Vector3 normal = data.Normal;

//				MarkerSpawner.SpawnMarker(
//					$"Prov_{data.ProvinceID}",
//					pos,
//					Quaternion.LookRotation(normal),
//					0.001f,
//					Color.yellow,
//					this.transform
//				);
//			}
//		}
//#endif
//	}
//}
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class PlanetMeta : MonoBehaviour
{
	[Header("Geometry Info")]
	public float SphereRadius = 1f;

	[Header("Data")]
	public PlanetMetaDataAsset DataAsset;

	[Header("Debug Adjustment")]
	public float DebugRotationX = 0f;
	public float DebugRotationY = 0f;

	public void Clear()
	{
		DataAsset?.Clear();
	}

	public void Register(int provinceID, Vector3 pos, Vector3 normal)
	{
		DataAsset?.Register(provinceID, pos, normal);
	}

	[ContextMenu("DEBUG: Spawn Markers for All Provinces")]
	public void DebugAllProvinces()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying && DataAsset != null)
		{
			var allData = DataAsset.GetAll();
			Quaternion rotation = Quaternion.Euler(DebugRotationX, DebugRotationY, 0f);

			foreach (var data in allData)
			{
				Vector3 localOffset = data.WorldPosition - transform.position;
				Vector3 rotatedPos = rotation * localOffset + transform.position;
				Vector3 rotatedNormal = rotation * data.Normal;

				MarkerSpawner.SpawnMarker(
					$"Prov_{data.ProvinceID}",
					rotatedPos,
					Quaternion.LookRotation(rotatedNormal),
					0.001f,
					Color.yellow,
					this.transform
				);
			}
		}
#endif
	}
}
