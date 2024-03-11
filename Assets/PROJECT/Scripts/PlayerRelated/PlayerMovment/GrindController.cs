using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Dreamteck;

public class GrindController : MonoBehaviour
{
    SplineFollower splineFollower;
    SplineProjector splineProjector;

    [SerializeField]float normalGrindSpeed;
    [SerializeField] float sprintSpeedMultiplier;
    [SerializeField] float SprintingTransitionSpeed;
  
    public bool isGrinding = false;
    bool isSpeedingUp;

    public static Action<bool> OnRailGrindStateChange;
    public static Action<JumpState> TriggerJumpingOffRails;

    private void OnEnable()
    {
        splineFollower = GetComponent<SplineFollower>();
        splineProjector = GetComponent<SplineProjector>();
        normalGrindSpeed = splineFollower.followSpeed;

        Jump.OnJumpStateChanged += GetOffRailsOnJump;
        Jump.GetExternalMomentum += JumpMomentumAddon;
    }

    private void OnDisable()
    {
        Jump.OnJumpStateChanged -= GetOffRailsOnJump;
        Jump.GetExternalMomentum -= JumpMomentumAddon;
    }

    private void Update()
    {
        if (splineFollower.spline == null) { splineFollower.follow = false; return; }

        transform.GetChild(0).gameObject.transform.rotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (splineFollower.direction == Spline.Direction.Forward)
            {
                float value = -splineFollower.followSpeed;
                splineFollower.followSpeed = value;
            }
            else
            {
                float value = Math.Abs(splineFollower.followSpeed);
                splineFollower.followSpeed = value;
            }
        }

        GrindRailSprinting();
        JumpOffAtTheEndOfRail();
    }

    private void GrindRailSprinting()
    {
        isSpeedingUp = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = isSpeedingUp ? normalGrindSpeed * sprintSpeedMultiplier : normalGrindSpeed;
        targetSpeed = splineFollower.direction == Spline.Direction.Forward ? targetSpeed : -targetSpeed;
        splineFollower.followSpeed = Mathf.Lerp(splineFollower.followSpeed, targetSpeed, SprintingTransitionSpeed * Time.deltaTime);
    }

    public void GoGrindOnThoseRails(SplineComputer splineToGrindOn)
    {
        if (isGrinding) return;

        splineFollower.spline = splineToGrindOn;
        splineFollower.RebuildImmediate();

        splineProjector.enabled = true;
        splineProjector.spline = splineToGrindOn;
        splineProjector.RebuildImmediate();
        splineProjector.CalculateProjection();

        double percent = splineProjector.GetPercent();
        splineFollower.SetPercent(percent);

        float dot = Vector3.Dot(splineFollower.result.position.normalized, transform.forward);
        if (splineFollower.direction == Spline.Direction.Forward)
        {
            if (dot > -0.58f && dot < -0.39f)
            {
                splineFollower.followSpeed = Mathf.Abs(normalGrindSpeed);
            }
            else
            {
                splineFollower.followSpeed = -normalGrindSpeed;
            }
        }
        else
        {
            if (dot > -0.6f && dot < 0.19)
            {
                splineFollower.followSpeed = Mathf.Abs(normalGrindSpeed);
            }
            else
            {
                splineFollower.followSpeed = -normalGrindSpeed;
            }
        }

        splineFollower.follow = true;
        isGrinding = true;
        OnRailGrindStateChange?.Invoke(isGrinding);
    }

    public void JumpOffAtTheEndOfRail()
    {
        if (splineFollower.direction == Spline.Direction.Forward)
        {
            if (splineFollower.result.percent > 0.97f)
            {
                TriggerJumpingOffRails?.Invoke(JumpState.inAir);
            }
        }
        else
        {
            if (splineFollower.result.percent < 0.05f)
            {
                TriggerJumpingOffRails?.Invoke(JumpState.inAir);
            }
        }
    }

    void ExitRails()
    {
        splineFollower.follow = !splineFollower.follow;
        splineFollower.spline = null;
        splineProjector.spline = null;
        splineProjector.enabled = false;
        isGrinding = false;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        OnRailGrindStateChange?.Invoke(splineFollower.follow);
    }

    private void GetOffRailsOnJump(JumpState _jumpStste)
    {
        if (_jumpStste == JumpState.Grounded || splineFollower.spline == null) return;
        ExitRails();
    }

    Vector3 JumpMomentumAddon()
    {
        if (isGrinding && isSpeedingUp)
        { return transform.forward * 3f; }

        else if (isGrinding && !isSpeedingUp)
        { return transform.forward * 2.5f; }

        return Vector3.zero;
    }
}