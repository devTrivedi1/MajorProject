using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5;

    [SerializeField] bool isPlayerGrinding = false;
    [SerializeField] Rigidbody rb;

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

    private void Update()
    {
        if (isPlayerGrinding) return;
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 cameraForwardDirection = Camera.main.transform.forward;
        cameraForwardDirection.y = 0;
        Vector3 cameraRightDirection = Camera.main.transform.right;

        Vector3 moveDirection = cameraForwardDirection * z + cameraRightDirection * x;
        moveDirection = moveDirection.normalized;

        Vector3 moveForce = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
        rb.velocity = moveForce;

        if(moveDirection != Vector3.zero)
        {
           transform.GetChild(0).forward = Vector3.Lerp(transform.GetChild(0).forward, moveDirection,Time.deltaTime*20f);
        }
        
    }

    public void SetIsPlayerGrinding(bool value)
    {
        isPlayerGrinding = value;
    }
}
