using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour
{
    [SerializeField] int damage = 1;
    Rigidbody rb;
    public Rigidbody Rb => rb;
    public bool manipulable { get; private set; } = true;

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
            rb.AddForce((targetable.transform.position - transform.position).normalized * throwForce, ForceMode.VelocityChange);
        }
        yield return new WaitUntil(() => manipulable);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!manipulable && collision.transform.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }
    }
}