using System.Collections.Generic;
using UnityEngine;
using CustomInspector;

public enum GravityDirection
{
    UpwardGravity,
    DownwardGravity,
    LeftwardGravity,
    RightwardGravity,
    ForwardGravity,
    BackwardGravity
}
public class Gravity : MonoBehaviour
{
    [SerializeField][SelfFill] Rigidbody rb;

    [HorizontalLine("GravityType", 2, FixedColor.BabyBlue)]
    [SerializeField] GravityDirection gravityDirection = GravityDirection.DownwardGravity;
    [SerializeField] ForceMode gravityForceMode = ForceMode.Acceleration;

    [HorizontalLine("GravitySettings", 2, FixedColor.BabyBlue)]
    [SerializeField] bool UseCustomGravity = true;
    [SerializeField] float CurrentGravityForce = 9.8f;

    [SerializeField] Dictionary<GravityDirection, Vector3> directionToGravitationalForce = new Dictionary<GravityDirection, Vector3>();


    private void Awake()
    {
        directionToGravitationalForce.Add(GravityDirection.UpwardGravity, Vector3.up);
        directionToGravitationalForce.Add(GravityDirection.DownwardGravity, Vector3.down);
        directionToGravitationalForce.Add(GravityDirection.LeftwardGravity, Vector3.left);
        directionToGravitationalForce.Add(GravityDirection.RightwardGravity, Vector3.right);
        directionToGravitationalForce.Add(GravityDirection.ForwardGravity, Vector3.forward);
        directionToGravitationalForce.Add(GravityDirection.BackwardGravity, Vector3.back);
    }
  
    private void FixedUpdate()
    {
        if (UseCustomGravity)
        {
            rb.AddForce(directionToGravitationalForce[gravityDirection] * CurrentGravityForce, gravityForceMode);
        }
    }

    void SetGravityForce(float _gravityForce) => CurrentGravityForce = _gravityForce;

    void SetGravityDirection(GravityDirection _gravityDirection) => gravityDirection = _gravityDirection;

    void SetGravityForceMode(ForceMode _forceMode) => gravityForceMode = _forceMode;


    private void OnEnable()
    {
        Jump.OnGravityChanged += SetGravityForce;
        Stomp.OnStompDown += SetGravityForce;
    }

    private void OnDisable()
    {
        Jump.OnGravityChanged -= SetGravityForce;
        Stomp.OnStompDown -= SetGravityForce;
    }
}
