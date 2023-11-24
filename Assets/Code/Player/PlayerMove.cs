using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 4.0f;

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
        MoveVelocity();

        HandleJumpVelocity();
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

    private void MoveVelocity() {
        Vector3 originalVelocity = playerRigidBody.velocity;
        playerRigidBody.velocity = new Vector3(velocity.x, originalVelocity.y + (velocity.y), velocity.z);
    }

    private void HandleMoveVelocity() {
        Vector3 move = transform.right * x + transform.forward * z;
        velocity = new Vector3(move.x, velocity.y, move.z);
    }

    private void HandleJumpVelocity() {
        if(Input.GetButton("Jump") && isGrounded) {
            Vector3 originalVelocity = playerRigidBody.velocity;
            playerRigidBody.velocity = new Vector3(originalVelocity.x, Mathf.Sqrt(jumpHeight * -2.0f * gravity), originalVelocity.z);
        }
    }
}
