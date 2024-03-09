using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour
{
    Rigidbody rb;
    public Rigidbody Rb => rb;
    public bool manipulable { get; private set; } = true;

    Vector3 lastExplosionPosition;
    float radius;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        manipulable = true;
    }

    public IEnumerator StopManipulation(float timer)
    {
        manipulable = false;
        float time = 0;
        while (time < timer)
        {
            time += Time.deltaTime;
            yield return null;
        }
        manipulable = true;
    }

    public virtual IEnumerator ApplyEffect(Targetable targetable, float throwForce) 
    {
        if (targetable.TryGetComponent(out Rigidbody rb))
        {
            //rb.AddForce((targetable.transform.position - transform.position).normalized * throwForce, ForceMode.VelocityChange);
            Utilities.KnockbackObjects(transform, 10f, throwForce, LayerMask.GetMask("Default"));
            lastExplosionPosition = transform.position;
            radius = 10f;
        }
        yield return new WaitUntil(() => manipulable);
        //gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(lastExplosionPosition, radius);
    }
#endif
}