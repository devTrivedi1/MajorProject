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
        GrindController.PlayerIsNowGrinding += SetIsPlayerGrinding;
    }

    private void OnDisable()
    {
        GrindController.PlayerIsNowGrinding -= SetIsPlayerGrinding;
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

        Vector3 direction = new Vector3(x, 0, z).normalized;
        rb.velocity = direction * speed;
    }

    public void SetIsPlayerGrinding(bool value)
    {
        isPlayerGrinding = value;
    }
}
