using UnityEngine;

public class TransformState : ComponentState
{
    Transform transform;
    Vector3 position;
    Quaternion rotation;
    Vector3 scale;
    Transform parent;
    
    public override void CaptureState(IResettable resettable)
    {
        transform = (resettable as IResettableTransform).transform;
        if (transform == null) { return; }
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;
        parent = transform.parent;
    }

    public override void ResetState()
    {
        if (transform == null) { return; }
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
        transform.parent = parent;
    }
}