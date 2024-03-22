using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour
{
    [SerializeField] protected int damage = 1;
    Rigidbody rb;
    public Rigidbody Rb => rb;
    public bool Thrown { get; private set; } = false;
    bool applied = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Thrown = false;
    }

    public void StopManipulation()
    {
        Thrown = true;
    }

    protected virtual void Effect(Targetable targetable = null, float throwForce = 0)
    {
        if (targetable != null)
        {
            rb.AddForce((targetable.transform.position - transform.position).normalized * throwForce, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Thrown)
        {
            if (collision.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
            if (!applied)
            {
                Effect(collision.transform.GetComponent<Targetable>());
                applied = true;
            }    
            gameObject.SetActive(false);
        }
    }
}