using UnityEngine;

public class RigidbodyState : ComponentState
{
    Rigidbody rb;
    Vector3 Velocity;
    bool Kinematic;
    RigidbodyConstraints RigidbodyConstraints;

    public override void CaptureState(IResettable resettable)
    {
        if (resettable is IResettableRb rigidbody && rigidbody.rb != null)
        {
            rb = rigidbody.rb;
            Velocity = rigidbody.rb.velocity;
            Kinematic = rigidbody.rb.isKinematic;
            RigidbodyConstraints = rigidbody.rb.constraints;
        }
    }

    public override void ResetState()
    {
        if (rb == null) { return; }
        rb.velocity = Velocity;
        rb.isKinematic = Kinematic;
        rb.constraints = RigidbodyConstraints;
    }
}