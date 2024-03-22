using UnityEngine;

public class BouncePad : EnviromentalAid
{
    public float jumpHeight = 10f;
    private GrindController _gc;

    protected override void OnFetchPlayerRefs(GrindController gc)
    {
        _gc = gc;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerRB != null)
        {
            if (_gc != null && _gc.isGrinding)
            {
                _gc.ExitRails();
                PerformJump(0.6f); // larger force multiplier for grinding!!!!!!!
            }
            else
            {
                RegularBounce();
            }
        }
    }

    void RegularBounce()
    {
        float forceMultiplier = (playerRB.velocity.magnitude > 1) ? 0.2f : 0.0f; 
        PerformJump(forceMultiplier);
    }

    void PerformJump(float forwardMultiplier)
    {
        float force = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y) * playerRB.mass);
        Vector3 forwardDirection = (playerRB.transform.forward * force * forwardMultiplier) + (Vector3.up * force);
        playerRB.AddForce(forwardDirection, ForceMode.Impulse);
    }
}
