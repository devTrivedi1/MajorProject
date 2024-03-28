using UnityEngine;

public class RigidbodyState : ComponentState
{
    Rigidbody rb;
    Vector3 velocity;
    Vector3 angularVelocity;
    bool kinematic;
    bool gravity;
    RigidbodyConstraints rigidbodyConstraints;

    public override void CaptureState(IResettable resettable)
    {
        if (resettable is IResettableRb rigidbody && rigidbody.rb != null)
        {
            rb = rigidbody.rb;
            velocity = rigidbody.rb.velocity;
            angularVelocity = rigidbody.rb.angularVelocity;
            kinematic = rigidbody.rb.isKinematic;
            rigidbodyConstraints = rigidbody.rb.constraints;
            gravity = rigidbody.rb.useGravity;
        }
    }

    public override void ResetState()
    {
        if (rb == null) { return; }
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
        rb.isKinematic = kinematic;
        rb.useGravity = gravity;
        rb.constraints = rigidbodyConstraints;
        rb.ResetInertiaTensor();
        rb.ResetCenterOfMass();
    }
}