using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 100.0f;
    [SerializeField] private float blendRate = 1.0f;
    [SerializeField] private Transform playerBody;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private float mouseX = 0.0f;
    private float mouseY = 0.0f;

    private float mouseLerpX = 0.0f;
    private float mouseLerpY = 0.0f;

    private void LateUpdate() {
        HandleLockInputs();
        GetMouseInput();
        RotatePlayer();
    }

    private void HandleLockInputs() {
        if(Input.GetButtonDown("Cancel")) Cursor.lockState = CursorLockMode.None;
    	if(Input.GetButtonDown("Click")) Cursor.lockState = CursorLockMode.Locked;
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
