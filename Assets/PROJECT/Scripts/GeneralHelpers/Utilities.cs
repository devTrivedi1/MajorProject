using System.Collections;
using UnityEngine;

public static class Utilities
{
    public static void KnockbackObjects(Transform origin, float radius, float knockbackStrength, int iterations, Collider[] objects)
    {
        foreach (Collider obj in objects)
        {
            if (obj.transform == origin) {  continue; }
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                ApplyForces(rb, origin.position, radius, knockbackStrength, iterations);
            }
        }
    }

    public static void KnockbackWithinRadius(Transform origin, float radius, float knockbackStrength, LayerMask mask, int iterations)
    {
        Collider[] objects = Physics.OverlapSphere(origin.position, radius, mask);
        KnockbackObjects(origin, radius, knockbackStrength, iterations, objects);
    }

    static IEnumerator ApplyForce(Rigidbody rb, Vector3 explosionOrigin, float explosionForce, float explosionRadius, int iterations)
    {
        int count = 0;
        while (count < iterations && rb != null)
        {
            if (rb != null)
            {
                float strength = Mathf.Lerp(1, 0.4f, Vector3.Distance(rb.position, explosionOrigin) / explosionRadius);
                Vector3 direction = (rb.position - explosionOrigin).normalized;
                rb.AddForce(explosionForce * strength * direction, ForceMode.VelocityChange);
            }
            count++;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    public static void ApplyForces(Rigidbody rb, Vector3 explosionOrigin, float explosionRadius, float explosionForce, int iterations)
    {
        if (CoroutineWorkHorse.instance == null)
        {
            (new GameObject("Coroutine WorkHorse")).AddComponent<CoroutineWorkHorse>();
        }
        CoroutineWorkHorse.instance.StartWork(ApplyForce(rb, explosionOrigin, explosionRadius, explosionForce, iterations));
    }
}


public class CoroutineWorkHorse : MonoBehaviour
{
    public static CoroutineWorkHorse instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartWork(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}