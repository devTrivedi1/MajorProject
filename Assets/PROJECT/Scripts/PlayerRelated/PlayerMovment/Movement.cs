using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] float tolerableSlopeAngle = 30;

    [SerializeField] bool isPlayerGrinding = false;
    [SerializeField] Rigidbody rb;

    float xInput;
    float zInput;
    Vector3 moveDirection;

    public static Action<Vector3> OnMoveDirectionChanged;

    private void OnEnable()
    {
        GrindController.OnRailGrindStateChange += SetIsPlayerGrinding;
        Jump.GetExternalMomentum += JumpMomentumAddon;
    }

    private void OnDisable()
    {
        GrindController.OnRailGrindStateChange -= SetIsPlayerGrinding;
        Jump.GetExternalMomentum -= JumpMomentumAddon;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isPlayerGrinding) return;
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        Vector3 cameraForwardDirection = Camera.main.transform.forward;
        cameraForwardDirection.y = 0;
        Vector3 cameraRightDirection = Camera.main.transform.right;

        moveDirection = cameraForwardDirection * zInput + cameraRightDirection * xInput;
        moveDirection = moveDirection.normalized;
        OnMoveDirectionChanged?.Invoke(moveDirection);
        Vector3 moveForce = new Vector3(moveDirection.x * speed, 0, moveDirection.z * speed);



        if (moveDirection != Vector3.zero)
        {
            Quaternion forwardRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, forwardRotation, Time.deltaTime * 20);        
        }
        rb.AddForce(moveForce, ForceMode.Acceleration);
    }

    public void SetIsPlayerGrinding(bool value)
    {
        isPlayerGrinding = value;
        OnPlayerOnRails();
    }

    Vector3 JumpMomentumAddon()
    {
        if (!isPlayerGrinding && moveDirection.magnitude > 0)
        {
            return transform.forward;
        }
        return Vector3.zero;
    }

    void OnPlayerOnRails()
    {
        if (isPlayerGrinding)
        {
            rb.interpolation = RigidbodyInterpolation.None;
        }
        else { rb.interpolation = RigidbodyInterpolation.Extrapolate; }
    }
}
