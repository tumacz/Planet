using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

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
		cameraClick.performed -= OnClick;
		cameraClick.Disable();
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
		if (!ctx.performed || cam == null)
			return;

		StartCoroutine(HandleClickNextFrame());
	}


	private IEnumerator HandleClickNextFrame()
	{
		yield return null;

		if (IsPointerOverUI())
			yield break;

		RaycastClick();
	}

	private bool IsPointerOverUI()
	{
		return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
	}

	private void RaycastClick()
	{
		Vector2 screenPos = Mouse.current.position.ReadValue();
		Ray ray = cam.ScreenPointToRay(screenPos);

		if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
		{
			var clickable = hit.collider.GetComponentInParent<IClickable>();
			if (clickable != null)
			{
				clickable.OnClicked(hit);
			}
			else
			{
				Debug.Log("Hit, but no IClickable implementation found.");
			}
		}
		else
		{
			Debug.Log("Raycast hit nothing.");
		}

	}
}
