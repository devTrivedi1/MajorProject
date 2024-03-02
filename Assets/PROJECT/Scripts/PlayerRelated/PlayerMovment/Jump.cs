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

    [HorizontalLine("Jump Settings", 2, FixedColor.Gray)]
    [SerializeField] float jumpForce = 5;
    [SerializeField] float momentumMultiplier = 2;
    [Layer][SerializeField] int layerMask;

    [HorizontalLine("Falling Settings", 2, FixedColor.Gray)]
    [SerializeField] float groundDetectionLength = 2f;
    [SerializeField] float fallMultiplier = 2.5f;

    [HorizontalLine("jump State", 2, FixedColor.Gray)]
    [ReadOnly][SerializeField] JumpState jumpState = JumpState.Grounded;

    public static Action<JumpState> OnJumpStateChanged;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && OnGround())
        {
            jumpState = JumpState.inAir;
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
            rb.velocity += new Vector3(0, jumpForce,0);
            jumpState = JumpState.Falling;
            OnJumpStateChanged?.Invoke(jumpState);
        }    
    }

    private bool OnGround()
    {
        if (Physics.OverlapSphere(transform.position, groundDetectionLength, 1 << layerMask).Length > 0)
        {
            jumpState = JumpState.Grounded;
            OnJumpStateChanged?.Invoke(jumpState);
            return true;
        }
        return false;
    }

    void FallingToGround()
    {
        if(rb.velocity.y<0 &&jumpState == JumpState.Falling)
        {
            rb.velocity -=  Vector3.up * fallMultiplier;
        }
    }

}