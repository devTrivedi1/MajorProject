using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
    private void Start()
    {
        Targeting.Instance.RegisterTarget(this);
    }

    private void OnDisable()
    {
        Targeting.Instance.UnregisterTarget(this);
    }
}