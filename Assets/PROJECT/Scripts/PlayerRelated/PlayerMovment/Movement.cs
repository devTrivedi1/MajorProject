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

        Vector3 direction = transform.right * x + transform.forward * z;
        direction = direction.normalized;
        Vector3 MoveForce = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
        rb.velocity = MoveForce;
  
    }

    public void SetIsPlayerGrinding(bool value)
    {
        isPlayerGrinding = value;
    }
}
