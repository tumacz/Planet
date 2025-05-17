using UnityEngine;

public static class ProvincePositionCalculator
{
    /// <summary>
    /// Przekszta�ca wsp�rz�dne UV prowincji na pozycj� w �wiecie, z offsetem od powierzchni planety.
    /// </summary>
    public static Vector3 GetWorldPositionFromUV(Vector2 uv, Transform planetTransform, float radius, float surfaceOffset = 0.0f)
    {
        float longitude = (1f - uv.x) * 360f - 180f;
        float latitude = uv.y * 180f - 90f;

        Vector3 dir = new Vector3(
            Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Sin(longitude * Mathf.Deg2Rad),
            Mathf.Sin(latitude * Mathf.Deg2Rad),
            Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Cos(longitude * Mathf.Deg2Rad)
        ).normalized;

        return planetTransform.TransformPoint(dir * (radius + surfaceOffset));
    }

    /// <summary>
    /// Zwraca normaln� planety (�wiatow�) w punkcie UV.
    /// </summary>
    public static Vector3 GetNormalFromUV(Vector2 uv, Transform planetTransform)
    {
        float longitude = (1f - uv.x) * 360f - 180f;
        float latitude = uv.y * 180f - 90f;

        Vector3 dir = new Vector3(
            Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Sin(longitude * Mathf.Deg2Rad),
            Mathf.Sin(latitude * Mathf.Deg2Rad),
            Mathf.Cos(latitude * Mathf.Deg2Rad) * Mathf.Cos(longitude * Mathf.Deg2Rad)
        ).normalized;

        return planetTransform.TransformDirection(dir);
    }
}