using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour, IResettable, IResettableTransform, IResettableRb, IResettableGO
{
    [SerializeField] protected int damage = 1;
    [Resettable] bool applied = false;
    [Resettable] bool thrown = false;
    Transform target;
    Rigidbody _rb;
    public Rigidbody rb => _rb;
    public bool Thrown => thrown;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        thrown = false;
    }

    public void EnableEffect(Transform target = null)
    {
        thrown = true;
        this.target = target;
    }

    protected virtual void Effect(Targetable targetable = null, float throwForce = 0)
    {
        if (targetable != null)
        {
            _rb.AddForce((targetable.transform.position - transform.position).normalized * throwForce, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrown && (target == null || target == collision.transform))
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