using UnityEngine;
using UnityEngine.InputSystem;

public class MouseControl : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Click Detection")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask planetLayer;

    [Header("Province Map")]
    [SerializeField] private ProvinceDatabase provinceDatabase;

    private InputAction cameraClick;

    private void OnEnable()
    {
        var input = GetComponent<PlayerInput>();
        var map = input.actions.FindActionMap("Camera");

        cameraClick = map.FindAction("Click");

        cameraClick.performed += OnClick;
        cameraClick.Enable();
    }

    private void OnDisable()
    {
        cameraClick.Disable();
        cameraClick.performed -= OnClick;
    }

    private void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (target == null)
        {
            Debug.LogWarning("No target assigned.");
        }
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, planetLayer))
        {
            Vector3 localDir = target.InverseTransformPoint(hit.point).normalized;

            float latitudeDeg = Mathf.Asin(localDir.y) * Mathf.Rad2Deg;
            float longitudeDeg = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

            float u = 1f - (longitudeDeg + 180f) / 360f;
            float v = (latitudeDeg + 90f) / 180f;
            Vector2 globalUV = new Vector2(u, v);

            Debug.Log($"Lat/Lon: ({latitudeDeg:F2}°, {longitudeDeg:F2}°)");
            Debug.Log($"Global UV: ({globalUV.x:F4}, {globalUV.y:F4})");

            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = renderer.material;
                Texture2D countriesMask = mat.GetTexture("_ProvinceMask") as Texture2D;

                if (countriesMask != null && countriesMask.isReadable)
                {
                    Color color = countriesMask.GetPixelBilinear(globalUV.x, globalUV.y);

                    int r = Mathf.RoundToInt(color.r * 255);
                    int g = Mathf.RoundToInt(color.g * 255);
                    int b = Mathf.RoundToInt(color.b * 255);
                    int id = (r << 16) | (g << 8) | b;

                    string hex = $"#{r:X2}{g:X2}{b:X2}";

                    Debug.Log($"RGB: ({r}, {g}, {b}) → HEX: {hex} → ID: {id}");

                    // Read province owner
                    if (provinceDatabase != null)
                    {
                        Province province = provinceDatabase.GetProvinceByColor(color);
                        if (province != null)
                        {
                            Debug.Log($"Province: {province.Name} | Owner: {province.Owner}");
                        }
                        else
                        {
                            Debug.Log("No province for this color.");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Teksture _CountriesMask unassigned in material.");
                }
            }
        }
    }
}
