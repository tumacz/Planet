using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseControl : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Click Detection")]
    [SerializeField] private Camera cam;

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

    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (cam == null) return;
        if(IsPointerOverUI()) return;// no click on UI

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            var clickable = hit.collider.GetComponentInParent<IClickable>();
            if (clickable != null)
            {
                clickable.OnClicked(hit);
            }
            else
            {
                Debug.Log("hit != null, no implementation from IClickable object.");
            }
        }
    }
}