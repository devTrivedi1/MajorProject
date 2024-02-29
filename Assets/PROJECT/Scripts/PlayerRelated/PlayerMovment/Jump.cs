using CustomInspector;
using MoreMountains.Tools;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class Jump : MonoBehaviour
{
    [SelfFill][SerializeField] Rigidbody rb;

    [SerializeField] float targetJumpForce = 5;
    [SerializeField] float jumpDuration = 1f;
    [SerializeField] bool isGrounded;
    [SerializeField] float groundDetectionLength = 2f;

    [HorizontalLine("Debug Info",2,FixedColor.Gray)]
    [Layer][SerializeField] int layerMask;
    [ReadOnly][SerializeField] float currentForce;
    [ReadOnly][SerializeField] float timer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            currentForce = 0;
            timer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded && timer < jumpDuration)
        {
            currentForce = Mathf.Lerp(0, targetJumpForce, timer / jumpDuration);
            rb.AddForce(Vector3.up * currentForce, ForceMode.Impulse);
            timer += Time.fixedDeltaTime;
        }
        else if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, groundDetectionLength, 1 << layerMask))
        {
            isGrounded = true;
        }
      
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, -transform.up * groundDetectionLength);
    }

}