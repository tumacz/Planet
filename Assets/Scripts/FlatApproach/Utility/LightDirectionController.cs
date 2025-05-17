using UnityEngine;

[ExecuteAlways]
public class LightDirectionController : MonoBehaviour
{
	public Vector3 LightDirection = new Vector3(0, -1, 0);

	public Material TargetMaterial;

	public Color GizmoColor = Color.yellow;
	public float GizmoLength = 1.0f;

	private void Update()
	{
		if (TargetMaterial != null)
		{
			TargetMaterial.SetVector("_LightDirection", LightDirection.normalized);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = GizmoColor;

		Vector3 start = transform.position;
		Vector3 dir = LightDirection.normalized * GizmoLength;

		Gizmos.DrawLine(start, start + dir);
		Gizmos.DrawSphere(start + dir, 0.05f);
	}
}