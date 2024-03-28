using UnityEngine;
using VInspector;

public class KnockbackThrowable : TelekineticObject
{
    [SerializeField] float forceStrength = 5f;
    [SerializeField] float knockbackRadius = 10f;
    [SerializeField] float dragOffset = 0.5f;
    [SerializeField] LayerMask layerMask;
    
    [Foldout("Debug")]
    [SerializeField] bool showRadius = false;
    Vector3 lastExplosionPosition;
    [EndFoldout]

    protected override void Effect(Targetable targetable = null, float throwForce = 0)
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, knockbackRadius, layerMask);
        PhysicsUtilities.KnockbackObjects(transform, knockbackRadius, forceStrength, dragOffset, objects);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].TryGetComponent(out IDamageable damageable)) { damageable.TakeDamage(damage); }
        }
        lastExplosionPosition = transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showRadius)
        {
            Gizmos.color = new(0, 1, 1, 0.2f);
            Gizmos.DrawSphere(transform.position, knockbackRadius);
        }

        if (lastExplosionPosition == Vector3.zero) { return; }  
        Gizmos.color = new(0, 1, 1, 0.2f);
        Gizmos.DrawSphere(lastExplosionPosition, knockbackRadius);
    }
#endif
}