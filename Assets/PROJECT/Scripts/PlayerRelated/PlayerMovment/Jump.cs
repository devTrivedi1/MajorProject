using CustomInspector;
using MoreMountains.Tools;
using UnityEngine;
using UnityRandom = UnityEngine.Random;


public enum JumpState
{
    Grounded,
    inAir,
    Falling
}


public class Jump : MonoBehaviour
{
    [SelfFill][SerializeField] Rigidbody rb;
    [SelfFill][SerializeField] Gravity customGravity;

    [HorizontalLine("Jump Settings", 2, FixedColor.Gray)]
    [SerializeField] float targetJumpForce = 5;
    [SerializeField] float jumpHeightDuration = 1f;

    [HorizontalLine("Falling Settings", 2, FixedColor.Gray)]
    [SerializeField] float groundDetectionLength = 2f;
    [SerializeField] float jumpFallDuration;

    [HorizontalLine("jump State", 2, FixedColor.Gray)]
    [ReadOnly][SerializeField] JumpState jumpState = JumpState.Grounded;

    [HorizontalLine("Debug Info", 2, FixedColor.Gray)]
    [Layer][SerializeField] int layerMask;
    [ReadOnly][SerializeField] float currentForce;
    [ReadOnly][SerializeField] float currentFallGravity;
    [ReadOnly][SerializeField] float timer;

    delegate void JumpStateFunction();

    private void Update()
    {
        GroundedToInAirFunctionality(GroundedToJump);
        FallingToGroundFunctionality(FallingToGround);
    }

    private void FixedUpdate()
    {
        InAirToFallingFunctionality(JumpThenFall);
    }

    void GroundedToInAirFunctionality(JumpStateFunction stateFunction)
    {
        if (jumpState == JumpState.Grounded)
        { stateFunction.Invoke(); }
    }

    void InAirToFallingFunctionality(JumpStateFunction stateFunction)
    {
        if (jumpState == JumpState.inAir)
        { stateFunction.Invoke(); }
    }

    void FallingToGroundFunctionality(JumpStateFunction stateFunction)
    {
        if (jumpState == JumpState.Falling)
        { stateFunction.Invoke(); }
    }

    private void GroundedToJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpState == JumpState.Grounded)
        {
            jumpState = JumpState.inAir;
            currentForce = 0;
            timer = 0;
            customGravity.UseCustomGravity = false;
        }
    }
    private void JumpThenFall()
    {
        if (timer < jumpHeightDuration)
        {
            currentForce = Mathf.Lerp(0, targetJumpForce, timer / jumpHeightDuration);
            rb.AddForce(Vector3.up * currentForce, ForceMode.Impulse);
            timer += Time.fixedDeltaTime;
        }
        else
        {
            jumpState = JumpState.Falling;
            timer = 0;
        }
    }

    private void FallingToGround()
    {
        if (timer < jumpFallDuration)
        {
            customGravity.UseCustomGravity = true;
            currentFallGravity = Mathf.Lerp(0, customGravity.TargetGravityForce, timer / jumpFallDuration);
            customGravity.CurrentGravityForce = currentFallGravity;
            timer += Time.deltaTime;
        }
        else if (Physics.Raycast(transform.position, -transform.up, groundDetectionLength, 1<<layerMask))
        {
            customGravity.UseCustomGravity = false;
            jumpState = JumpState.Grounded;
        }
    }





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, -transform.up * groundDetectionLength);
    }

}