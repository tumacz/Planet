using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Camera))]
public class CameraFlyOverStage : MonoBehaviour
{
	[Header("Target")]
	[SerializeField] private Transform targetQuad;
	[SerializeField] private float margin = 5f;

	[Header("Movement Settings")]
	[SerializeField] private float moveSpeed = 20f;
	[SerializeField] private float moveSmoothTime = 0.15f;
	[SerializeField] private float maxDistanceSpeed = 50f;
	[SerializeField] private float minDistanceSpeed = 2f;

	[Header("Zoom Settings")]
	[SerializeField] private float zoomSpeed = 100f;
	[SerializeField] private float zoomSmoothTime = 0.25f;
	[SerializeField] private float minZoom = -80f;
	[SerializeField] private float maxZoom = -4f;

	[Header("Tilt Settings")]
	[SerializeField] private bool enableTilt = true;
	[SerializeField] private float tiltStartZoom = -25f;
	[SerializeField] private float tiltEndZoom = -4f;
	[SerializeField] private float minTiltAngle = 0f;
	[SerializeField] private float maxTiltAngle = -10f;

	private Vector2 moveInput;
	private Vector2 currentMoveInput;
	private Vector2 moveVelocity;

	private float scrollInput;
	private float targetZoom;
	private float zoomVelocity;

	private InputAction cameraMove;
	private InputAction cameraZoom;

	private Coroutine flyRoutine;

	private float minX, maxX, minY, maxY;

	private Camera cam;

	private void OnEnable()
	{
		var input = GetComponent<PlayerInput>();
		var map = input.actions.FindActionMap("Camera");

		cameraMove = map.FindAction("CameraFly");
		cameraZoom = map.FindAction("CameraScroll");

		cameraMove.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
		cameraMove.canceled += _ => moveInput = Vector2.zero;

		cameraZoom.performed += ctx => scrollInput = ctx.ReadValue<float>();
		cameraZoom.canceled += _ => scrollInput = 0f;

		cameraMove.Enable();
		cameraZoom.Enable();

		cam = GetComponent<Camera>();

		if (targetQuad != null)
			CalculateQuadBounds();

		targetZoom = transform.position.z;
		flyRoutine = StartCoroutine(FlyLoop());
	}

	private void OnDisable()
	{
		cameraMove.Disable();
		cameraZoom.Disable();

		if (flyRoutine != null)
			StopCoroutine(flyRoutine);
	}

	private void CalculateQuadBounds()
	{
		Renderer quadRenderer = targetQuad.GetComponent<Renderer>();
		if (quadRenderer == null)
		{
			Debug.LogError("Target Quad has no Renderer attached!");
			return;
		}

		Bounds bounds = quadRenderer.bounds;

		minX = bounds.min.x - margin;
		maxX = bounds.max.x + margin;
		minY = bounds.min.y - margin;
		maxY = bounds.max.y + margin;
	}

	private IEnumerator FlyLoop()
	{
		while (true)
		{
			if (targetQuad != null)
			{
				currentMoveInput = Vector2.SmoothDamp(currentMoveInput, moveInput, ref moveVelocity, moveSmoothTime);

				Vector3 position = transform.position;

				targetZoom += scrollInput * zoomSpeed * Time.deltaTime;
				targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
				position.z = Mathf.SmoothDamp(position.z, targetZoom, ref zoomVelocity, zoomSmoothTime);

				float zoomFactor = Mathf.InverseLerp(minZoom, maxZoom, position.z);
				float adjustedMoveSpeed = Mathf.Lerp(maxDistanceSpeed, minDistanceSpeed, zoomFactor);

				position.y += currentMoveInput.y * adjustedMoveSpeed * Time.deltaTime;
				position.x -= currentMoveInput.x * adjustedMoveSpeed * Time.deltaTime;

				position.x = Mathf.Clamp(position.x, minX, maxX);
				position.y = Mathf.Clamp(position.y, minY, maxY);

				transform.position = position;

				if (enableTilt)
				{
					float tiltFactor = Mathf.InverseLerp(tiltStartZoom, tiltEndZoom, position.z);
					float tiltAngle = Mathf.Lerp(minTiltAngle, maxTiltAngle, tiltFactor);

					Vector3 eulerRotation = transform.rotation.eulerAngles;
					transform.rotation = Quaternion.Euler(tiltAngle, eulerRotation.y, eulerRotation.z);
				}
			}

			yield return null;
		}
	}
}