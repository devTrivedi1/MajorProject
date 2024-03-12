using System.Collections;
using UnityEngine;

public class KnockbackThrowable : TelekineticObject
{
    [SerializeField] float forceStrength = 5f;
    [SerializeField] float knockbackRadius = 10f;
    [SerializeField] LayerMask layerMask;
    [SerializeField] int iterations = 3;

    Vector3 lastExplosionPosition;

    public override IEnumerator ApplyEffect(Targetable targetable, float throwForce)
    {
        if (targetable.TryGetComponent(out Rigidbody rb))
        {
            Collider[] objects = Physics.OverlapSphere(transform.position, knockbackRadius, layerMask);
            Utilities.KnockbackObjects(transform, knockbackRadius, forceStrength, iterations, objects);
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].TryGetComponent(out IDamageable damageable)) { damageable.TakeDamage(damage); }
            }
            lastExplosionPosition = transform.position;
        }
        gameObject.SetActive(false);
        yield return new WaitUntil(() => manipulable);
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