using UnityEngine;

[ExecuteAlways]
public class TerrainFaceTile : MonoBehaviour
{
	public void Setup(Vector3 localUp, int resolution, int tiles, Material mat)
	{
		Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
		Vector3 axisB = Vector3.Cross(localUp, axisA);

		for (int y = 0; y < tiles; y++)
		{
			for (int x = 0; x < tiles; x++)
			{
				float step = 1f / tiles;

				Vector2 percentMin = new Vector2(x, y) * step;
				Rect uvRect = new Rect(percentMin.x, percentMin.y, step, step);

				GameObject tile = new GameObject($"Tile_{x}_{y}");
				tile.transform.parent = transform;
				tile.transform.localPosition = Vector3.zero;

				var meshTile = tile.AddComponent<MeshTile>();
				meshTile.Build(localUp, axisA, axisB, resolution, uvRect, mat);
			}
		}
	}
}