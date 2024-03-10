using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5;

    [SerializeField] bool isPlayerGrinding = false;
    [SerializeField] Rigidbody rb;

    float xInput;
    float zInput;
    Vector3 moveDirection;

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

        Vector3 moveForce = new Vector3(moveDirection.x * speed, 0, moveDirection.z * speed);

        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.fixedDeltaTime * 20f);
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
       
        if (!isPlayerGrinding && moveDirection.magnitude>0)
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
