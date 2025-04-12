using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class CameraOrbitController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Zoom & Distance")]
    [SerializeField] private float distance = 10f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomSmoothTime = 0.25f;

    [Header("Orbit")]
    [SerializeField] private float moveSmoothTime = 0.15f;

    [Header("Latitude Limits")]
    [SerializeField] private float minLatitude = -89f;
    [SerializeField] private float maxLatitude = 89f;

    private float latitude = 0f;
    private float longitude = 0f;

    // Input
    private Vector2 moveInput;
    private Vector2 currentMoveInput;
    private Vector2 moveVelocity;

    private float scrollInput;
    private float targetDistance;
    private float zoomVelocity;

    // Input System
    private InputAction cameraFly;
    private InputAction cameraScroll;

    private Coroutine orbitRoutine;

    private void OnEnable()
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

        targetDistance = distance;

        orbitRoutine = StartCoroutine(OrbitLoop());
    }

    private void OnDisable()
    {
        cameraFly.Disable();
        cameraScroll.Disable();

        if (orbitRoutine != null)
            StopCoroutine(orbitRoutine);
    }

    private void Start()
    {
        PositionCameraAtStart();
    }

    private void PositionCameraAtStart()
    {
        if (target == null) return;

        float scale = Mathf.Max(target.localScale.x, target.localScale.y, target.localScale.z);
        float minDistance = scale * 1.02f;
        float maxDistance = scale * 1.7f;

        distance = targetDistance = Mathf.Lerp(minDistance, maxDistance, 0.5f);

        latitude = 20f;
        longitude = 45f;

        float latRad = latitude * Mathf.Deg2Rad;
        float lonRad = longitude * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(latRad) * Mathf.Sin(lonRad),
            Mathf.Sin(latRad),
            Mathf.Cos(latRad) * Mathf.Cos(lonRad)
        );

        transform.position = target.position + offset * distance;
        transform.LookAt(target.position);

        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
    }

    private IEnumerator OrbitLoop()
    {
        while (true)
        {
            if (target != null)
            {
                float scale = target.localScale.x;
                float minDistance = scale * 1.02f;
                float maxDistance = scale * 1.7f;

                // Scroll → targetDistance
                targetDistance -= scrollInput * zoomSpeed * Time.deltaTime;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

                // Smooth zoom
                distance = Mathf.SmoothDamp(distance, targetDistance, ref zoomVelocity, zoomSmoothTime);

                // Orbit speed based on distance
                float orbitSpeed = Mathf.Lerp(0.7f, 18f, Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance)));

                // Smooth WASD movement
                currentMoveInput = Vector2.SmoothDamp(currentMoveInput, moveInput, ref moveVelocity, moveSmoothTime);

                // Update angles
                longitude += currentMoveInput.x * orbitSpeed * Time.deltaTime;
                latitude += currentMoveInput.y * orbitSpeed * Time.deltaTime;
                latitude = Mathf.Clamp(latitude, minLatitude, maxLatitude);

                // Convert to position
                float latRad = latitude * Mathf.Deg2Rad;
                float lonRad = longitude * Mathf.Deg2Rad;

                Vector3 offset = new Vector3(
                    Mathf.Cos(latRad) * Mathf.Sin(lonRad),
                    Mathf.Sin(latRad),
                    Mathf.Cos(latRad) * Mathf.Cos(lonRad)
                );

                transform.position = target.position + offset * distance;
                transform.LookAt(target.position);

                // Lock roll (Z rotation)
                Vector3 euler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
            }

            yield return null;
        }
    }
}