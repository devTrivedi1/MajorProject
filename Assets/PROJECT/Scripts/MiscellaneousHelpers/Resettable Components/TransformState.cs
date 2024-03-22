using UnityEngine;

public class TransformState : ComponentState
{
    Transform transform;
    Vector3 Position;
    Quaternion Rotation;
    Vector3 Scale;
    
    public override void CaptureState(IResettable resettable)
    {
        transform = (resettable as IResettableTransform).transform;
        if (transform == null) { return; }
        Position = transform.position;
        Rotation = transform.rotation;
        Scale = transform.localScale;
    }

    public override void ResetState()
    {
        if (transform == null) { return; }
        transform.position = Position;
        transform.rotation = Rotation;
        transform.localScale = Scale;
    }
}