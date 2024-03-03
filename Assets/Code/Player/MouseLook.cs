using UnityEngine;
using System;

public class MouseLook : MonoBehaviour
{
    public static MouseLook Instance { get; private set; }
    private PauseEventSystem pauseEventSystem;

    [SerializeField] private float blendRate = 0.5f;
    [SerializeField] private Transform playerBody;

    private Camera mouseCamera;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private float mouseX = 0.0f;
    private float mouseY = 0.0f;

    private float mouseLerpX = 0.0f;
    private float mouseLerpY = 0.0f;

    public float XRotation { 
        get { return xRotation; }
    }

    public float YRotation { 
        get { return yRotation; }
    }

    private bool isEnabled = true;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        pauseEventSystem = PauseEventSystem.Instance;
        mouseCamera = GetComponent<Camera>();
    }

    private void Update() {
        UpdateCameraFOV();
        HandleLockInputs();
    }

    private void LateUpdate() {
        if(!isEnabled) return;

        GetMouseInput();
        RotatePlayer();
    }

    public void SetupRotation(object sender, EventArgs e) {
        xRotation = PlayerStorage.XRotation;
        yRotation = PlayerStorage.YRotation;

        transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        playerBody.Rotate(Vector3.up * yRotation);
    }

    private void UpdateCameraFOV() {
        mouseCamera.fieldOfView = GameSettings.FOV;
    }

    public void Enable(object sender, EventArgs e) {
        isEnabled = true;
    }

    public void Disable(object sender, EventArgs e) {
        isEnabled = false;
    }

    public void ReleaseCursor(object sender, EventArgs e) {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor(object sender, EventArgs e) {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleLockInputs() {
        if(Input.GetButtonDown("Cancel")) pauseEventSystem.InvokePauseToggle();
    }

    private void GetMouseInput() {
        mouseX = Input.GetAxis("Mouse X") * (0.25f + (6.0f * (GameSettings.Sensitivity / 100.0f)));
        mouseY = Input.GetAxis("Mouse Y") * (0.25f + (6.0f * (GameSettings.Sensitivity / 100.0f)));
    }

    private void RotatePlayer() {
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        yRotation += mouseX;

        mouseLerpX = Mathf.Lerp(mouseLerpX, xRotation, blendRate);
        mouseLerpY = Mathf.Lerp(mouseLerpY, mouseX, blendRate);

        transform.localRotation = Quaternion.Euler(mouseLerpX, 0.0f, 0.0f);
        playerBody.Rotate(Vector3.up * mouseLerpY);
    }
}
