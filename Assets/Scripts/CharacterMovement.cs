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
    private float moveValue;
    private float movementInput;
    private float smoothingVelocity;
    private float jumpBufferTimeLeft = -1;
    private float jumpCoyoteTimeLeft = -1;
    private float jumpMoveValue;
    private float jumpMovementInput;
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
        moveValue = controls.Player.Move.ReadValue<float>();

        ProcessGrounded();
        ProcessFlip();
        ProcessMovementInput();

        var horizontalVelocity = movementInput * speed;
        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);

        if (RequiredJump && CanJump)
        {
            Jump();
        }

        jumpBufferTimeLeft -= Time.fixedDeltaTime;
        jumpCoyoteTimeLeft -= Time.fixedDeltaTime;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        groundedThisFrame =
            other.contacts.Any(contact => Vector2.Angle(contact.normal, Vector2.up) <= GroundedMaxAngle);
    }

    private void ProcessMovementInput()
    {
        if (Grounded)
        {
            movementInput = Mathf.SmoothDamp(movementInput, moveValue,
                ref smoothingVelocity, inputSmoothTime);
        }
        else if (moveValue != 0 && airControl > 0)
        {
            movementInput = Mathf.SmoothDamp(movementInput, moveValue,
                ref smoothingVelocity, inputSmoothTime / airControl);
            if (jumpMoveValue > 0)
            {
                movementInput = Mathf.Min(movementInput, jumpMovementInput);
            }
            else if (jumpMoveValue < 0)
            {
                movementInput = Mathf.Max(movementInput, jumpMovementInput);
            }
        }
    }

    private void ProcessGrounded()
    {
        framesToBeGroundedLeft = groundedThisFrame ? framesToBeGroundedLeft - 1 : FramesToBeGrounded;
        if (Grounded)
        {
            jumpCoyoteTimeLeft = jumpCoyoteTime;
        }

        groundedThisFrame = false;
    }

    private void ProcessFlip()
    {
        if (moveValue != 0)
        {
            var scale = transform.localScale;
            scale.x = moveValue;
            transform.localScale = scale;
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce);
        jumpBufferTimeLeft = -1;
        jumpCoyoteTimeLeft = -1;
        jumpMoveValue = moveValue;
        jumpMovementInput = movementInput;
        framesToBeGroundedLeft = FramesToBeGrounded;
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