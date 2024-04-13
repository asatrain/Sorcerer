using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float inputSmoothTime;
    [SerializeField] private float jumpForce;
    [SerializeField] private float airControl;

    private const float GroundedMaxAngle = 50;

    private Controls controls;
    private float movementInput;
    private float smoothingVelocity;
    private bool jumpRequired;
    private bool grounded;

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
        if (grounded)
        {
            jumpRequired = true;
        }
    }

    private void FixedUpdate()
    {
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

        if (jumpRequired && grounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
            jumpRequired = false;
        }
        
        grounded = false;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        grounded = other.contacts.Any(contact => Vector2.Angle(contact.normal, Vector2.up) <= GroundedMaxAngle);
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