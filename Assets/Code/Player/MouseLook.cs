using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseLook : MonoBehaviour
{
    public static MouseLook Instance { get; private set; }
    private PauseEventSystem pauseEventSystem;

    [SerializeField] private float sensitivity = 100.0f;
    [SerializeField] private float blendRate = 1.0f;
    [SerializeField] private Transform playerBody;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private float mouseX = 0.0f;
    private float mouseY = 0.0f;

    private float mouseLerpX = 0.0f;
    private float mouseLerpY = 0.0f;

    private bool isEnabled = true;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        pauseEventSystem = PauseEventSystem.Instance;
    }

    private void LateUpdate() {
        HandleLockInputs();
        if(!isEnabled) return;

        GetMouseInput();
        RotatePlayer();
    }

    public void Enable(object sender, EventArgs e) {
        isEnabled = true;
    }

    public void Disable(object sender, EventArgs e) {
        isEnabled = false;
    }

    public void ReleaseCursor(object sender, EventArgs e) {
        Cursor.lockState = CursorLockMode.None;
    }

    public void LockCursor(object sender, EventArgs e) {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleLockInputs() {
        if(Input.GetButtonDown("Cancel")) pauseEventSystem.InvokePauseToggle();
    }

    private void GetMouseInput() {
        mouseX = Input.GetAxis("Mouse X") * sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;
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
