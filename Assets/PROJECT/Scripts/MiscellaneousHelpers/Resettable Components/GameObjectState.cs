using UnityEngine;

public class GameObjectState : ComponentState
{
    bool active;
    GameObject gameObject;

    public override void CaptureState(IResettable resettable)
    {
        gameObject = (resettable as IResettableGO).gameObject;
        if (gameObject == null) { return; }
        active = gameObject.activeSelf;
    }

    public override void ResetState()
    {
        if (gameObject == null) { return; }
        gameObject.SetActive(active);
    }
}