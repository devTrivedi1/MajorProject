using UnityEngine;
using System;

public class EnemyHealth : ObjectHealth
{
    [SerializeField] ParticleSystem deathVfx;
    public static Action<GameObject> OnEnemyDeath;

    protected override void ObjectDeath()
    {
        if (deathVfx != null)
        {
            ParticleSystem particle = Instantiate(deathVfx, transform.position + Vector3.up, Quaternion.identity);
            Destroy(particle, 3f);
        }
        OnEnemyDeath?.Invoke(gameObject);
        Destroy(gameObject);
    }
}
