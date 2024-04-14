using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Serialization;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float inputSmoothTime;
    [SerializeField] private float jumpForce;
    [SerializeField] private float airControl;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float jumpCoyoteTime;

    private const float GroundedMaxAngle = 50;
    private const int FramesToBeGrounded = 2;

    private Controls controls;
    private float movementInput;
    private float smoothingVelocity;
    private float jumpBufferTimeLeft = -1;
    private float jumpCoyoteTimeLeft = -1;
    private int framesToBeGroundedLeft = FramesToBeGrounded;
    private bool groundedThisFrame;

    private bool Grounded => framesToBeGroundedLeft <= 0;
    private bool CanJump => Grounded || jumpCoyoteTimeLeft >= 0;
    private bool RequiredJump => jumpBufferTimeLeft >= 0;

    private void Awake()
    {
        controls = new Controls();
        controls.Player.Jump.performed += JumpOnPerformed;
    }

    private void Start()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        jumpBufferTimeLeft = jumpBufferTime;
    }

    private void FixedUpdate()
    {
        framesToBeGroundedLeft = groundedThisFrame ? framesToBeGroundedLeft - 1 : FramesToBeGrounded;
        if (Grounded)
        {
            jumpCoyoteTimeLeft = jumpCoyoteTime;
        }
        
        var moveValue = controls.Player.Move.ReadValue<float>();
        if (moveValue != 0)
        {
            var scale = transform.localScale;
            scale.x = moveValue;
            transform.localScale = scale;
        }

        movementInput = Mathf.SmoothDamp(movementInput, moveValue, ref smoothingVelocity, inputSmoothTime);
        var horizontalVelocity = movementInput * speed;
        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);

        if (RequiredJump && CanJump)
        {
            Jump();
        }
        
        jumpBufferTimeLeft -= Time.fixedDeltaTime;
        jumpCoyoteTimeLeft -= Time.fixedDeltaTime;
        groundedThisFrame = false;
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce);
        jumpBufferTimeLeft = -1;
        jumpCoyoteTimeLeft = -1;
        framesToBeGroundedLeft = FramesToBeGrounded;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        groundedThisFrame = other.contacts.Any(contact => Vector2.Angle(contact.normal, Vector2.up) <= GroundedMaxAngle);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}