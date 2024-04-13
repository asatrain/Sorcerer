using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float inputSmoothTime;

    private Controls controls;
    private float movementInput;
    private float smoothingVelocity;

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
        throw new NotImplementedException();
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
        rb.velocity = new Vector2(movementInput * speed, rb.velocity.y);
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