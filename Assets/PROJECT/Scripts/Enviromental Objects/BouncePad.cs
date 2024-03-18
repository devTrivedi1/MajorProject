using UnityEngine;

public class BouncePad : MonoBehaviour, IEnviromentalAids
{
    public float jumpHeight = 10f;
    private Rigidbody playerRB;

    public void SetTargetRigidbody(Rigidbody rb)
    {
        playerRB = rb;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.TryGetComponent(out GrindController GC);

        if (playerRB != null)
        {
            if (GC.isGrinding == true)
            {

                GC.ExitRails();
                //Grinding+Jumppad == BEEG JUMP
                float force = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y) * playerRB.mass);
                Vector3 forwardDirection = playerRB.transform.forward;
                playerRB.AddForce(Vector3.up * force + forwardDirection * force * 0.6f, ForceMode.Impulse);
                
            }
            else
            {
                RegularBounce();
            }
        }
    }

    void RegularBounce()
    {
        if (playerRB.velocity.magnitude > 1)
        {
            float force = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y) * playerRB.mass);
            Vector3 forwardDirection = playerRB.transform.forward;
            playerRB.AddForce(Vector3.up * force + forwardDirection * force * 0.2f, ForceMode.Impulse);
        }
        else
        {
            float force = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y) * playerRB.mass);
            playerRB.AddForce(Vector3.up * force, ForceMode.Impulse);
        }
    }
}
