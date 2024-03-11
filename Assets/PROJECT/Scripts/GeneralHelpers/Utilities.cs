using System.Collections;
using UnityEngine;

public static class Utilities
{
    public static void KnockbackObjects(Transform origin, float radius, float knockbackStrength, LayerMask mask)
    {
        Collider[] objects = Physics.OverlapSphere(origin.position, radius, mask);
        Debug.Log(objects.Length);
        foreach (Collider obj in objects)
        {
            if (obj.transform == origin) {  continue; }
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                //float strength = Mathf.Lerp(1, 0, Vector3.Distance(rb.position, origin.position) / radius);
                //rb.AddForce((rb.position - origin.position).normalized * knockbackStrength * strength, ForceMode.VelocityChange);
                ApplyForces(rb, rb.position, origin.position, radius, knockbackStrength);
            }
        }
    }

    static IEnumerator ApplyForce(Rigidbody rb, Vector3 rbPosition, Vector3 explosionOrigin, float explosionForce, float explosionRadius)
    {
        float timer = 0;
        while (timer < 0.1f && rb != null)
        {
            float strength = Mathf.Lerp(1, 0.4f, Vector3.Distance(rbPosition, explosionOrigin) / explosionRadius);
            timer += Time.deltaTime;
            Vector3 direction = (rbPosition - explosionOrigin).normalized;
            direction.y = 0;
            rb.AddForce(explosionForce * strength * direction, ForceMode.VelocityChange);
            yield return null;
        }
    }

    public static void ApplyForces(Rigidbody rb, Vector3 rbPosition, Vector3 explosionOrigin, float explosionRadius, float explosionForce)
    {
        if (CoroutineWorkHorse.instance == null)
        {
            (new GameObject("Coroutine WorkHorse")).AddComponent<CoroutineWorkHorse>();
        }
        CoroutineWorkHorse.instance.StartWork(ApplyForce(rb, rbPosition, explosionOrigin, explosionRadius, explosionForce));
    }
}


public class CoroutineWorkHorse : MonoBehaviour
{
    public static CoroutineWorkHorse instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void StartWork(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}