using UnityEngine;

[DisallowMultipleComponent]
public class ProvinceInteractor : MonoBehaviour, IClickable
{
	[Header("References")]
	[SerializeField] private Camera cam;
	[SerializeField] private ProvinceDatabase provinceDatabase;
	[SerializeField] private Transform planetTransform;

	[Header("Click Settings")]
	[SerializeField] private LayerMask globeLayer;

	[Header("Marker Settings")]
	[SerializeField, Range(0.001f, 0.2f)] private float markerScale = 0.2f;
	[SerializeField] private Color clickMarkerColor = Color.magenta;

	private void Awake()
	{
		if (cam == null)
			cam = Camera.main;
	}

	public void OnClicked(RaycastHit hit)
	{
		Vector3 pos = hit.point;
		Vector3 normal = hit.normal;
		Quaternion rot = Quaternion.LookRotation(normal);

		// Spawn marker using utility
		MarkerSpawner.SpawnMarker(
			"Klik",
			pos,
			rot,
			markerScale,
			clickMarkerColor,
			this.transform // optional parent
		);

		// Read UV coordinates
		Vector2 uv = hit.textureCoord;
		var renderer = hit.collider?.GetComponent<Renderer>();
		var mat = renderer?.sharedMaterial;
		var provinceMap = mat?.GetTexture("_ProvinceMap") as Texture2D;

		if (provinceMap == null || !provinceMap.isReadable)
		{
			Debug.LogWarning("ProvinceMap texture not readable.");
			return;
		}

		Color provinceColor = provinceMap.GetPixelBilinear(uv.x, uv.y);
		var province = provinceDatabase.GetProvinceByColor(provinceColor);

		if (province != null)
		{
			Debug.Log($"Kliknięto prowincję: {province.Name}, UV: {uv}");
		}
		else
		{
			Debug.LogWarning($"Nie znaleziono prowincji dla koloru: {provinceColor}");
		}
	}
}
