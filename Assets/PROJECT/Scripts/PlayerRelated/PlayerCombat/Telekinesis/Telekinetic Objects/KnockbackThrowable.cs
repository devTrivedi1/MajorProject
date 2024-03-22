using System.Collections;
using UnityEngine;

public class KnockbackThrowable : TelekineticObject
{
    [SerializeField] float forceStrength = 5f;
    [SerializeField] float knockbackRadius = 10f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] int iterations = 3;

    Vector3 lastExplosionPosition;

    protected override void Effect(Targetable targetable = null, float throwForce = 0)
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, knockbackRadius, layerMask);
        PhysicsUtilities.KnockbackObjects(transform, knockbackRadius, forceStrength, iterations, objects);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].TryGetComponent(out IDamageable damageable)) { damageable.TakeDamage(damage); }
        }
        lastExplosionPosition = transform.position;
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