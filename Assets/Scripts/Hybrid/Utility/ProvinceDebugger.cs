//using UnityEngine;

//public class ProvinceDebugger : MonoBehaviour, IClickable
//{
//    [SerializeField] private Transform planetTransform;
//    [SerializeField] private ProvinceDatabase provinceDatabase;

//    [SerializeField, Range(0.45f, 0.65f)] private float surfaceOffset = 0.5f;
//    [SerializeField, Range(0.001f, 0.015f)] private float markerScale = 0.002f;

//    private void OnValidate()
//    {
//        surfaceOffset = Mathf.Max(0f, surfaceOffset);
//        markerScale = Mathf.Clamp(markerScale, 0.0001f, 1f);
//    }

//    public void OnClicked(RaycastHit hit)
//    {
//        if (provinceDatabase == null || planetTransform == null)
//        {
//            Debug.LogWarning("[ProvinceDebugger] Missing references.");
//            return;
//        }

//        // Read color from the texture
//        Renderer renderer = hit.collider.GetComponent<Renderer>();
//        if (renderer == null) return;

//        Texture2D mask = renderer.material.GetTexture("ProvinceMap") as Texture2D;
//        if (mask == null || !mask.isReadable)
//        {
//            Debug.LogWarning("[ProvinceDebugger] Province mask missing or unreadable.");
//            return;
//        }

//        Vector3 localDir = planetTransform.InverseTransformPoint(hit.point).normalized;
//        float latitudeDeg = Mathf.Asin(localDir.y) * Mathf.Rad2Deg;
//        float longitudeDeg = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

//        float u = 1f - (longitudeDeg + 180f) / 360f;
//        float v = (latitudeDeg + 90f) / 180f;
//        Vector2 globalUV = new Vector2(u, v);

//        Color color = mask.GetPixelBilinear(globalUV.x, globalUV.y);

//        Province province = provinceDatabase.GetProvinceByColor(color);
//        if (province == null)
//        {
//            Debug.Log("[ProvinceDebugger] Province not found.");
//            return;
//        }

//        Debug.Log($"[ProvinceDebugger] Province: {province.Name}, Owner: {province.Owner}");

//        // 🥔 Potato Spawn 🥔
//        float radius = planetTransform.localScale.x * 0.5f;

//        Vector3 spawnPosition = ProvincePositionCalculator.GetWorldPositionFromUV(
//            province.UVCenter,
//            planetTransform,
//            radius,
//            surfaceOffset
//        );

//        Quaternion rotation = Quaternion.LookRotation(
//            (spawnPosition - planetTransform.position).normalized
//        );

//        SpawnPotato(province.Name, spawnPosition, rotation, markerScale);
//    }

//    private void SpawnPotato(string name, Vector3 position, Quaternion rotation, float scale)
//    {
//        GameObject potato = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        potato.name = $"Ziemniak_{name}";
//        potato.transform.position = position;
//        potato.transform.rotation = rotation;
//        potato.transform.localScale = Vector3.one * scale;
//        potato.GetComponent<Renderer>().material.color = Color.magenta;
//    }
//}