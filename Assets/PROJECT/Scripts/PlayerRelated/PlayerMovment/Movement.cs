using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5;

    [SerializeField] bool isPlayerGrinding = false;
    [SerializeField] Rigidbody rb;

    float xInput;
    float zInput;

    private void OnEnable()
    {
        GrindController.OnRailGrindStateChange += SetIsPlayerGrinding;
    }

    private void OnDisable()
    {
        GrindController.OnRailGrindStateChange -= SetIsPlayerGrinding;
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

        Vector3 moveDirection = cameraForwardDirection * zInput + cameraRightDirection * xInput;
        moveDirection = moveDirection.normalized;

        Vector3 moveForce = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
        rb.velocity = moveForce;

        if (moveDirection != Vector3.zero)
        {
            transform.GetChild(0).forward = Vector3.Lerp(transform.GetChild(0).forward, moveDirection, Time.fixedDeltaTime * 20f);
        }
    }

    public void SetIsPlayerGrinding(bool value)
    {
        isPlayerGrinding = value;
        OnPlayerOnRails();
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
