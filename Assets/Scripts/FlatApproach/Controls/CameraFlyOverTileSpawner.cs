using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Camera))]
public class CameraFlyOverTileSpawner : MonoBehaviour
{
	[Header("Target")]
	[SerializeField] private Transform targetSpawner;
	[SerializeField] private float margin = 5f;

	[Header("Movement Settings")]
	[SerializeField] private float moveSmoothTime = 0.15f;
	[SerializeField] private float maxDistanceSpeed = 2f;
	[SerializeField] private float minDistanceSpeed = 0.3f;

	[Header("Zoom Settings")]
	[SerializeField] private float zoomSpeed = 15f;
	[SerializeField] private float zoomSmoothTime = 0.25f;
	[SerializeField] private float minZoom = -80f;
	[SerializeField] private float maxZoom = -4f;

	[Header("Tilt Settings")]
	[SerializeField] private bool enableTilt = true;
	[SerializeField] private float minTiltAngle = 0f;
	[SerializeField] private float maxTiltAngle = -10f;
	[SerializeField, Range(0f, 1f)] private float tiltStartPercent = 0.7f;

	private Vector2 moveInput;
	private Vector2 currentMoveInput;
	private Vector2 moveVelocity;

	private float scrollInput;
	private float targetZoom;
	private float zoomVelocity;

	private float tiltVelocity;

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

		if (targetSpawner != null)
			CalculateSpawnerBounds();

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

	private void CalculateSpawnerBounds()
	{
		TileSpawner spawner = targetSpawner.GetComponent<TileSpawner>();
		if (spawner == null)
		{
			Debug.LogError("Target does not have TileSpawner component!");
			return;
		}

		Vector3 scale = targetSpawner.lossyScale;

		float width = (spawner.columns - 1) * spawner.spacing * scale.x;
		float height = (spawner.rows - 1) * spawner.spacing * scale.y;

		// Obliczanie poprawnego œrodka mapy
		Vector3 center = targetSpawner.position + new Vector3(width / 2f, height / 2f, 0f);

		minX = center.x - width / 2f - margin;
		maxX = center.x + width / 2f + margin;
		minY = center.y - height / 2f - margin;
		maxY = center.y + height / 2f + margin;
	}

	private IEnumerator FlyLoop()
	{
		while (true)
		{
			if (targetSpawner != null)
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
					float zoomPercent = Mathf.InverseLerp(minZoom, maxZoom, targetZoom);

					float tiltProgress = Mathf.InverseLerp(tiltStartPercent, 1f, zoomPercent);
					tiltProgress = Mathf.Clamp01(tiltProgress);

					float targetTiltAngle = Mathf.Lerp(minTiltAngle, maxTiltAngle, tiltProgress);

					float currentTiltAngle = transform.rotation.eulerAngles.x;
					float smoothTilt = Mathf.SmoothDampAngle(currentTiltAngle, targetTiltAngle, ref tiltVelocity, zoomSmoothTime);

					Vector3 eulerRotation = transform.rotation.eulerAngles;
					transform.rotation = Quaternion.Euler(smoothTilt, eulerRotation.y, eulerRotation.z);
				}
			}

			yield return null;
		}
	}
}