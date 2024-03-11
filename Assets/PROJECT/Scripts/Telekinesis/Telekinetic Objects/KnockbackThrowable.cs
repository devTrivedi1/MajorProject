using System.Collections;
using UnityEngine;

public class KnockbackThrowable : TelekineticObject
{
    [SerializeField] float forceStrengthModifier = 5f;
    [SerializeField] float knockbackRadius = 10f;
    [SerializeField] LayerMask layerMask;

    Vector3 lastExplosionPosition;

    public override IEnumerator ApplyEffect(Targetable targetable, float throwForce)
    {
        if (targetable.TryGetComponent(out Rigidbody rb))
        {
            Utilities.KnockbackObjects(transform, knockbackRadius, throwForce * forceStrengthModifier, layerMask);
            lastExplosionPosition = transform.position;
        }
        yield return new WaitUntil(() => manipulable);
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (lastExplosionPosition == Vector3.zero) { return; }  
        Gizmos.color = new(0, 1, 1, 0.2f);
        Gizmos.DrawSphere(lastExplosionPosition, knockbackRadius);
    }
#endif
}