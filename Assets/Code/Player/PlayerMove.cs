using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float jumpHeight = 4.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float horizontalDragFactor = 0.5f;

    [SerializeField] private Transform playerContainerTransform;
    [SerializeField] private Rigidbody playerRigidBody;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isWalking;
    private bool isEnabled = true;

    public bool IsWalking {
        get { return isWalking; }
    }

    private float x;
    private float z;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }


    private void Update() {
        if(!isEnabled) return;
        GetInput();

        HandleMoveVelocity();
        HandleJumpVelocity();

        AdjustDrag();
    }

    public void Enable(object sender, EventArgs e) {
        isEnabled = true;
        playerRigidBody.isKinematic = !isEnabled;
    }

    public void Disable(object sender, EventArgs e) {
        isEnabled = false;
        playerRigidBody.isKinematic = !isEnabled;
    }

    public void UpdateIsGrounded(object sender, bool value) {
        isGrounded = value;
    }

    public void TeleportToSpawn(object sender, EventArgs e) {
        playerContainerTransform.position = WorldSpawner.GetPlayerSpawnLocation();
    }

    private void GetInput() {
        x = Input.GetAxis("Horizontal") * speed;
        z = Input.GetAxis("Vertical") * speed;
        
        isWalking = x != 0.0f || z != 0.0f && isGrounded;
    }

    private void HandleMoveVelocity() {
        Vector3 move = transform.right * x + transform.forward * z;
        playerRigidBody.AddForce(move, ForceMode.VelocityChange);
    }

    private void HandleJumpVelocity() {
        if(Input.GetButton("Jump") && isGrounded) {
            Vector3 originalVelocity = playerRigidBody.velocity;
            playerRigidBody.velocity = new Vector3(originalVelocity.x, Mathf.Sqrt(jumpHeight * -2.0f * gravity), originalVelocity.z);
        }
    }

    private void AdjustDrag() {
        Vector3 velocity = playerRigidBody.velocity;

        velocity.x *= 1.0f - horizontalDragFactor;
        velocity.z *= 1.0f - horizontalDragFactor;

        playerRigidBody.velocity = velocity;
    }
}
