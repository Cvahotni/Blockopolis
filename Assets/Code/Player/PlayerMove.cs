using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    [SerializeField] private float walkSpeed = 4.325f;
    [SerializeField] private float underWaterWalkSpeed = 2.125f;

    [SerializeField] private float drag = 0.25f;
    [SerializeField] private float underWaterDrag = 12.0f;

    [SerializeField] private float jumpHeight = 4.0f;
    [SerializeField] private float underWaterJumpHeight = 4.0f;
    [SerializeField] private float gravity = -9.81f;
    
    [SerializeField] private Transform playerContainerTransform;
    [SerializeField] private Rigidbody playerRigidBody;

    private Vector3 velocity;
    private float speed;

    private bool isGrounded;
    private bool isWalking;
    private bool isUnderWater;

    private bool allowsInput = true;
    private bool isEnabled = true;

    private Vector3 lastPosition;

    public bool IsWalking {
        get { return isWalking; }
    }

    private float x;
    private float z;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        lastPosition = transform.position;
    }

    private void Update() {
        if(!isEnabled) return;

        if(allowsInput) {
            GetInput();
            HandleJump();
        }

        HandleMoveVelocity();
        lastPosition = transform.position;
    }

    public void Enable(object sender, EventArgs e) {
        isEnabled = true;
        playerRigidBody.isKinematic = !isEnabled;
    }

    public void Disable(object sender, EventArgs e) {
        isEnabled = false;
        playerRigidBody.isKinematic = !isEnabled;
    }

    public void AllowInput(object sender, EventArgs e) {
        allowsInput = true;
    }

    public void DenyInput(object sender, EventArgs e) {
        allowsInput = false;
        isWalking = false;

        x = 0.0f;
        z = 0.0f;
    }

    public void UpdateIsGrounded(object sender, bool value) {
        isGrounded = value;
    }

    public void TeleportToSpawn(object sender, EventArgs e) {
        playerContainerTransform.position = WorldSpawner.GetPlayerSpawnLocation();
    }

    public void EnterWater(object sender, EventArgs e) {
        speed = underWaterWalkSpeed;
        playerRigidBody.drag = underWaterDrag;
        isUnderWater = true;
    }

    public void ExitWater(object sender, EventArgs e) {
        speed = walkSpeed;
        playerRigidBody.drag = drag;
        isUnderWater = false;
    }

    private void GetInput() {
        x = Input.GetAxis("Horizontal") * speed;
        z = Input.GetAxis("Vertical") * speed;
        
        isWalking = x != 0.0f || z != 0.0f && isGrounded && Vector3.Distance(lastPosition, transform.position) >= 1.0f / (Time.frameCount / Time.time);
    }

    private void HandleMoveVelocity() {
        Vector3 move = transform.right * x + transform.forward * z;
        playerRigidBody.velocity = new Vector3(move.x, playerRigidBody.velocity.y, move.z);
    }

    private void HandleJump() {
        if(Input.GetButton("Jump") && (isGrounded || isUnderWater)) {
            float currentJumpHeight = isUnderWater ? underWaterJumpHeight : jumpHeight;

            Vector3 originalVelocity = playerRigidBody.velocity;
            playerRigidBody.velocity = new Vector3(originalVelocity.x, Mathf.Sqrt(currentJumpHeight * -2.0f * gravity), originalVelocity.z);
        }
    }
}
