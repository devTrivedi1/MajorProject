using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [HideInInspector] public Movement player;
    public Rigidbody rb;
    public int speed = 5;
    public float lifetime = 5f;
    public int damage = 1;
    public bool isExplosive = false;
    [SerializeField] ParticleSystem impactVfx;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void InitializeStats(int _damage, int _speed, float _lifeTime)
    {
        speed = _speed;
        lifetime = _lifeTime;
        damage = _damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            if(!isExplosive)
            {
                Destroy(gameObject);
            }

            if (impactVfx == null) return;
            ParticleSystem particle = Instantiate(impactVfx, transform.position, Quaternion.identity);
            Destroy(particle, particle.main.duration + 1f);
        }
        else
        {
            Destroy(gameObject);
            if (impactVfx == null) return;
            ParticleSystem particle = Instantiate(impactVfx, transform.position, Quaternion.identity);
            Destroy(particle, particle.main.duration + 1f);
        }
    }

}
