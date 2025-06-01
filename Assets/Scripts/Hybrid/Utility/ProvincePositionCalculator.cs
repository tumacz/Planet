//using UnityEngine;

//public static class ProvincePositionCalculator
//{
//	public static Vector3 GetDirectionFromUV(Vector2 uv)
//	{
//		float longitude = (1f - uv.x) * 360f - 180f;
//		float latitude = uv.y * 180f - 90f;

//		return new Vector3(
//			Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Sin(longitude * Mathf.Deg2Rad),
//			Mathf.Sin(latitude * Mathf.Deg2Rad),
//			Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Cos(longitude * Mathf.Deg2Rad)
//		).normalized;
//	}

//	// Stara metoda mo¿e zostaæ, ale ju¿ nie u¿ywana w tym podejœciu
//	public static Vector3 GetWorldPositionFromUV(Vector2 uv, Transform planetTransform, float radius, float surfaceOffset = 0.0f)
//	{
//		Vector3 dir = GetDirectionFromUV(uv);
//		return planetTransform.position + dir * (radius + surfaceOffset);
//	}
//}
