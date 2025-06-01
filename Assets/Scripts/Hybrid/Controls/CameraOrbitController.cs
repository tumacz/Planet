using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class CameraOrbitController : MonoBehaviour
{
	[Header("Target")]
	[SerializeField] private Transform target;

	[Header("Distance Scaling")]
	[Tooltip("Multiplier applied to planet radius to determine base distance.")]
	[SerializeField, Min(0.1f)] private float distanceMultiplier = 1.5f;

	[Header("Zoom Settings")]
	[Tooltip("Min and max zoom relative to calculated base distance.")]
	[SerializeField] private float zoomRangeMin = 0.85f;
	[SerializeField] private float zoomRangeMax = 1.15f;

	[SerializeField] private float zoomSpeed = 5f;
	[SerializeField] private float zoomSmoothTime = 0.25f;

	[Header("Orbit Speed")]
	[SerializeField] private float orbitSpeedMin = 2f;
	[SerializeField] private float orbitSpeedMax = 6f;

	[Header("Movement Smoothing")]
	[SerializeField] private float moveSmoothTime = 0.15f;

	[Header("Latitude Limits")]
	[SerializeField] private float minLatitude = -89f;
	[SerializeField] private float maxLatitude = 89f;

	// Internal state
	private float planetRadius = 1f;
	private float latitude = 20f;
	private float longitude = 45f;

	private float targetDistance;
	private float distance;
	private float zoomVelocity;

	private Vector2 moveInput;
	private Vector2 currentMoveInput;
	private Vector2 moveVelocity;
	private float scrollInput;

	private InputAction cameraFly;
	private InputAction cameraScroll;
	private Coroutine orbitRoutine;

	private void OnEnable()
	{
		SetupInput();
		orbitRoutine = StartCoroutine(OrbitLoop());
	}

	private void OnDisable()
	{
		cameraFly?.Disable();
		cameraScroll?.Disable();

		if (orbitRoutine != null)
			StopCoroutine(orbitRoutine);
	}

	private void Start()
	{
		AlignToTarget();
	}

	private void SetupInput()
	{
		var input = GetComponent<PlayerInput>();
		var map = input.actions.FindActionMap("Camera");

		cameraFly = map.FindAction("CameraFly");
		cameraScroll = map.FindAction("CameraScroll");

		cameraFly.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
		cameraFly.canceled += _ => moveInput = Vector2.zero;

		cameraScroll.performed += ctx => scrollInput = ctx.ReadValue<float>();
		cameraScroll.canceled += _ => scrollInput = 0f;

		cameraFly.Enable();
		cameraScroll.Enable();
	}

	public void AlignToTarget()
	{
		if (target == null) return;

		if (target.TryGetComponent(out PlanetMeta meta))
		{
			planetRadius = meta.SphereRadius;
		}

		latitude = 20f;
		longitude = 45f;

		float baseDistance = planetRadius * distanceMultiplier;
		float minDistance = baseDistance * zoomRangeMin;
		float maxDistance = baseDistance * zoomRangeMax;

		distance = targetDistance = Mathf.Lerp(minDistance, maxDistance, 0.5f);
	}

	private IEnumerator OrbitLoop()
	{
		while (true)
		{
			if (target != null)
				ApplyOrbiting();

			yield return null;
		}
	}

	private void ApplyOrbiting()
	{
		float baseDistance = planetRadius * distanceMultiplier;
		float minDistance = baseDistance * zoomRangeMin;
		float maxDistance = baseDistance * zoomRangeMax;

		// Zoom
		targetDistance -= scrollInput * zoomSpeed * Time.deltaTime;
		targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
		distance = Mathf.SmoothDamp(distance, targetDistance, ref zoomVelocity, zoomSmoothTime);

		// Orbit speed
		float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
		float orbitSpeed = Mathf.Lerp(orbitSpeedMin, orbitSpeedMax, t);

		// Input smoothing
		currentMoveInput = Vector2.SmoothDamp(currentMoveInput, moveInput, ref moveVelocity, moveSmoothTime);

		longitude += currentMoveInput.x * orbitSpeed * Time.deltaTime;
		latitude += currentMoveInput.y * orbitSpeed * Time.deltaTime;
		latitude = Mathf.Clamp(latitude, minLatitude, maxLatitude);

		// Spherical to Cartesian
		transform.position = target.position + GetOrbitOffset(latitude, longitude) * distance;
		transform.LookAt(target.position);

		// Lock roll
		var euler = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
	}

	private static Vector3 GetOrbitOffset(float latitude, float longitude)
	{
		float latRad = latitude * Mathf.Deg2Rad;
		float lonRad = longitude * Mathf.Deg2Rad;

		return new Vector3(
			Mathf.Cos(latRad) * Mathf.Sin(lonRad),
			Mathf.Sin(latRad),
			Mathf.Cos(latRad) * Mathf.Cos(lonRad)
		);
	}
}