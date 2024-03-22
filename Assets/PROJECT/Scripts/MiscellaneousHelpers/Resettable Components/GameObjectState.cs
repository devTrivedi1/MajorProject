using UnityEngine;

public class GameObjectState : ComponentState
{
    bool Active;
    GameObject gameObject;

    public override void CaptureState(IResettable resettable)
    {
        gameObject = (resettable as IResettableGO).gameObject;
        if (gameObject == null) { return; }
        Active = gameObject.activeSelf;
    }

    public override void ResetState()
    {
        if (gameObject == null) { return; }
        gameObject.SetActive(Active);
    }
}