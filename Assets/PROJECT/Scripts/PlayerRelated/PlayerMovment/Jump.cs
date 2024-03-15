using CustomInspector;
using MoreMountains.Tools;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using System;


public enum JumpState
{
    Grounded,
    inAir,
    Falling
}


public class Jump : MonoBehaviour
{
    [SelfFill][SerializeField] Rigidbody rb;
    [SelfFill][SerializeField] MMGizmo jumpGizmo;

    [HorizontalLine("Jump Settings", 2, FixedColor.Gray)]
    [SerializeField] float verticalForce = 5;
    [SerializeField] float momentumMultiplier = 2;

    [HorizontalLine("Falling Settings", 2, FixedColor.Gray)]
    [Range(0, -1)][SerializeField] float groundDetectionOffset = 0.5f;
    Vector3 groundDetectionOffsetVector;
    [SerializeField] float groundDetectionRadius = 2f;
    [SerializeField] float fallMultiplier = 2.5f;
   
    [HorizontalLine("Layer To Trigger Jump", 2, FixedColor.Gray)]
    [Layer][SerializeField] int layerMask;

    [HorizontalLine("Debug Stats", 2, FixedColor.Gray)]
    [ReadOnly][SerializeField]Vector3 ExternalMomentum;
    [ReadOnly][SerializeField] JumpState jumpState = JumpState.Grounded;

    public static Action<JumpState> OnJumpStateChanged;
    public static Func<Vector3> GetExternalMomentum;

    private void OnEnable()
    {
        GrindController.TriggerJumpingOffRails += SetJumpStateTo;

    }

    private void OnDisable()
    {
        GrindController.TriggerJumpingOffRails -= SetJumpStateTo;
    }


    private void OnValidate()
    {

        groundDetectionOffsetVector = new Vector3(0, groundDetectionOffset, 0);
        if (jumpGizmo != null)
        {
            jumpGizmo.GizmoType = MMGizmo.GizmoTypes.Position;
            jumpGizmo.PositionMode = MMGizmo.PositionModes.Sphere;
            jumpGizmo.PositionSize = groundDetectionRadius;
            jumpGizmo.GizmoOffset = groundDetectionOffsetVector;
            jumpGizmo.DisplayText = true;
            jumpGizmo.TextMode = MMGizmo.TextModes.CustomText;
            jumpGizmo.TextToDisplay = "Ground Detection";

        }
    }

    private void Update()
    {
        GroundDetection();
        if (Input.GetKeyDown(KeyCode.Space) && jumpState == JumpState.Grounded)
        {
            SetJumpStateTo(JumpState.inAir);
            ExternalMomentum += GetExternalMomentum();
            OnJumpStateChanged?.Invoke(jumpState);
           
        }
    }

    private void FixedUpdate()
    {
        DoAJump();
        FallingToGround();
    }

    void DoAJump()
    {
        if (jumpState == JumpState.inAir)
        {
            if(ExternalMomentum == Vector3.zero)
            {
                ExternalMomentum = GetExternalMomentum();
            }
            rb.velocity +=  (Vector3.up * verticalForce) + (ExternalMomentum *momentumMultiplier);
            SetJumpStateTo(JumpState.Falling);
            OnJumpStateChanged?.Invoke(jumpState);
        }
    }

    void FallingToGround()
    {
        if(jumpState == JumpState.Grounded) return;
        if (rb.velocity.y < 0)
        {
            rb.velocity -= Vector3.up * fallMultiplier;
        } 
    }

    private void GroundDetection()
    {
        if (jumpState == JumpState.inAir) return;

        if (Physics.OverlapSphere(transform.position + groundDetectionOffsetVector, groundDetectionRadius, 1 << layerMask).Length > 0)
        {
            SetJumpStateTo(JumpState.Grounded);
            OnJumpStateChanged?.Invoke(jumpState);
            ExternalMomentum = Vector3.zero;
        }
        else
        {
            SetJumpStateTo(JumpState.Falling);
        }
    }

    void SetJumpStateTo(JumpState state)
    {
        jumpState = state;
    }
}