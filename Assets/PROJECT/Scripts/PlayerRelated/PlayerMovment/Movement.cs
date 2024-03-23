using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CustomInspector;

public class Movement : MonoBehaviour
{
    [SelfFill][SerializeField] Rigidbody rb;

    [HorizontalLine("Ground Movement Info", 3, FixedColor.Gray)]
    [SerializeField] float speed = 5;
    [ReadOnly][SerializeField] bool isPlayerGrinding = false;

    [HorizontalLine("Slope Movement Info", 3, FixedColor.Gray)]
    [SerializeField] float tolerableSlopeAngle = 30;
    [SerializeField] float downSlopeGravity = 30;
    [SerializeField] float upSlopeSpeed = 1.25f;
    [SerializeField] AnimationCurve slopeCurve;
    [SerializeField] float slopeAlignDuration = 0.5f;
    [ReadOnly][SerializeField] bool onSlope = false;
    float slopeLerpFactor;

    float xInput;
    float zInput;

    Vector3 moveDirection;
    Vector3 moveForce;

    float time;

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
        moveForce = new Vector3(moveDirection.x * speed, 0, moveDirection.z * speed);

        if (moveDirection != Vector3.zero)
        {
            Quaternion forwardRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, forwardRotation, Time.deltaTime * 20);
            SlopePhysics();
        }

        rb.AddForce(moveForce, ForceMode.Acceleration);
    }



    void SlopePhysics()
    {
        RaycastHit hit;
        float slopeAngle = 0;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
        {
            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            onSlope = slopeAngle < tolerableSlopeAngle && slopeAngle != 0;
        }

        if (!onSlope) { time = 0; return; }

        moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;
        moveDirection.y = 0;

        if (rb.velocity.y < 0)
        {
            rb.AddForce(-hit.normal * downSlopeGravity, ForceMode.Acceleration);
            moveForce = moveDirection * speed * upSlopeSpeed *1.5f;
        }

        moveForce = moveDirection * speed * upSlopeSpeed;
        RotationSlopeAlignment(hit);
    }

    private void RotationSlopeAlignment(RaycastHit hit)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        if (time < slopeAlignDuration)
        {
            time += Time.deltaTime;
            slopeLerpFactor = slopeCurve.Evaluate(time / slopeAlignDuration);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, slopeLerpFactor);
        }
        else { transform.rotation = targetRotation; }
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
