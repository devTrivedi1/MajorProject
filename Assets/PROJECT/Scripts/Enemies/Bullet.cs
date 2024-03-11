using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [ReadOnly][SerializeField] int speed = 5;
    [ReadOnly][SerializeField] float lifetime = 5f;
    [ReadOnly][SerializeField] int damage = 1;
    [SerializeField] ParticleSystem impactVfx;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public void InitializeBullet(int _damage, int _speed, float _lifeTime)
    {
        speed = _speed;
        lifetime = _lifeTime;
        damage = _damage;
    }

    void FixedUpdate()
    {
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);

            Destroy(gameObject);

            if (impactVfx == null) return;
            ParticleSystem particle = Instantiate(impactVfx, transform.position, Quaternion.identity);
            Destroy(particle, particle.main.duration + 1f);
        }
    }
}
