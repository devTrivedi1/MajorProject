using CustomInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityRandom = UnityEngine.Random;


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
    public GravityDirection gravityDirection = GravityDirection.DownwardGravity;
    public ForceMode gravityForceMode = ForceMode.Acceleration;

    [HorizontalLine("GravitySettings", 2, FixedColor.BabyBlue)]
    [SerializeField] float targetGravityForce = 9.8f;
    public bool UseCustomGravity = true;
    [ReadOnly] public float CurrentGravityForce = 9.8f;

    public float TargetGravityForce => targetGravityForce;
    [SerializeField] Dictionary<GravityDirection, Vector3> directionToGravitationalForce = new Dictionary<GravityDirection, Vector3>();


    private void OnEnable()
    {
        directionToGravitationalForce.Add(GravityDirection.UpwardGravity, Vector3.up);
        directionToGravitationalForce.Add(GravityDirection.DownwardGravity, Vector3.down);
        directionToGravitationalForce.Add(GravityDirection.LeftwardGravity, Vector3.left);
        directionToGravitationalForce.Add(GravityDirection.RightwardGravity, Vector3.right);
        directionToGravitationalForce.Add(GravityDirection.ForwardGravity, Vector3.forward);
        directionToGravitationalForce.Add(GravityDirection.BackwardGravity, Vector3.back);

        CurrentGravityForce = targetGravityForce;
    }

    private void FixedUpdate()
    {
        if (UseCustomGravity)
        {
            rb.AddForce(directionToGravitationalForce[gravityDirection] * CurrentGravityForce, gravityForceMode);
        }
    }

}