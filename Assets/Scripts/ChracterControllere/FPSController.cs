using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    public Transform cameraRoot;

    public float cameraPosY = 1.7f;
    public float mouseSensitivity = 200f;
    public float minLookAngle = -80f;
    public float maxLookAngle = 80f;

    public float moveSpeed = 5f;

    private float xRotation = 0f;
    private Vector3 fixedCameraPosition;

    private PlayerInputActions input;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool isCursorLocked = true;

    private void Awake()
    {
        input = new PlayerInputActions();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += _ => lookInput = Vector2.zero;

        fixedCameraPosition = new Vector3(0f, cameraPosY, 0f);
        cameraRoot.localPosition = fixedCameraPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleCursor();
        }

        if (isCursorLocked)
            HandleLook();

        HandleMove();
        cameraRoot.localPosition = fixedCameraPosition;
    }

    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMove()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    void ToggleCursor()
    {
        isCursorLocked = !isCursorLocked;

        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLocked;
    }
}