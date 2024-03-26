using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : EnemyBase
{

    bool isFiring = false;
    [SerializeField] ParticleSystem muzzleFlashVfx;

    void Update()
    {
        if (PlayerInRange())
        {
            if (!isFiring)
            {
                StartCoroutine(FireRoutine());
            }
        }
    }

    IEnumerator FireRoutine()
    {
        isFiring = true;

        while (PlayerInRange())
        {
            Fire();
            yield return new WaitForSeconds(1f / fireRate);
        }

        isFiring = false;
    }

    void Fire()
    {
        GameObject _bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        _bullet.GetComponent<Bullet>().InitializeBullet(projectileDamage, projectileSpeed, projectileLifetime);

        if (muzzleFlashVfx != null)
        {
            muzzleFlashVfx.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, playerRange);
    }
}