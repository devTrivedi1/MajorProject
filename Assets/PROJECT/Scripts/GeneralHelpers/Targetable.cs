using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
    private void Awake()
    {
        Targeting.RegisterTarget(this);
    }

    private void OnDisable()
    {
        Targeting.UnregisterTarget(this);
    }
}