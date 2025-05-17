using UnityEngine;

[ExecuteAlways]
public class TileChunk : MonoBehaviour
{
	public Texture2D heightMap;
	public Rect uvRect;

	private void Start()
	{
		UpdateMeshSubdivider();
	}

	public void Setup(Texture2D heightMap, Rect uvRect)
	{
		this.heightMap = heightMap;
		this.uvRect = uvRect;

		UpdateMeshSubdivider();
	}

	private void UpdateMeshSubdivider()
	{
		MeshSubdivider subdivider = GetComponent<MeshSubdivider>();
		if (subdivider != null)
		{
			subdivider.heightMap = heightMap;
			subdivider.uvRect = uvRect;
			subdivider.Regenerate();
		}
	}
}