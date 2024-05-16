using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2 : MonoBehaviour
{
    public float speed = 5f;
    public Transform cameraTransform;
    private Rigidbody _rb;
    private Vector2 _moveInput;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 move = cameraTransform.forward * _moveInput.y + cameraTransform.transform.right * _moveInput.x; move.y = 0f;
        _rb.AddForce(move.normalized *speed, ForceMode.VelocityChange);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
}
