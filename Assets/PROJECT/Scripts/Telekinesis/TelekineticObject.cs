using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour
{
    [SerializeField] protected int damage = 1;
    Rigidbody rb;
    public Rigidbody Rb => rb;
    public bool Thrown { get; private set; } = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Thrown = false;
    }

    public void StopManipulation()
    {
        Thrown = true;
    }

    public virtual IEnumerator ApplyEffect(Targetable targetable, float throwForce) 
    {
        if (targetable.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce((targetable.transform.position - transform.position).normalized * throwForce, ForceMode.VelocityChange);
        }
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Thrown)
        {
            if (collision.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
            gameObject.SetActive(false);
        }
    }
}