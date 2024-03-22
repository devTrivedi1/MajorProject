using UnityEngine;

public class RigidbodyState : ComponentState
{
    Rigidbody rb;
    Vector3 Velocity;
    Vector3 AngularVelocity;
    bool Kinematic;
    RigidbodyConstraints RigidbodyConstraints;

    public override void CaptureState(IResettable resettable)
    {
        if (resettable is IResettableRb rigidbody && rigidbody.rb != null)
        {
            rb = rigidbody.rb;
            Velocity = rigidbody.rb.velocity;
            AngularVelocity = rigidbody.rb.angularVelocity;
            Kinematic = rigidbody.rb.isKinematic;
            RigidbodyConstraints = rigidbody.rb.constraints;
        }
    }

    public override void ResetState()
    {
        if (rb == null) { return; }
        rb.velocity = Velocity;
        rb.angularVelocity = AngularVelocity;
        rb.isKinematic = Kinematic;
        rb.constraints = RigidbodyConstraints;
        rb.ResetInertiaTensor();
        rb.ResetCenterOfMass();
    }
}