using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    Movement player;

    [HorizontalLine("Turret Activation Settings ", 2, FixedColor.CherryRed)]
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float playerRange = 10f; // Range within which players are detected

    [HorizontalLine("References ", 2, FixedColor.CherryRed)]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform aimHolder;
    [SerializeField] Transform firePoint;
    [SerializeField] ParticleSystem muzzleFlashVfx;

    [HorizontalLine("Projectile Settings ", 2, FixedColor.CherryRed)]
    [SerializeField] int bulletSpeed = 5;
    [SerializeField] float bulletLifetime = 5f;
    [SerializeField] int bulletDamage = 1;

    private bool isFiring = false;

    void Start()
    {
        player = FindObjectOfType<Movement>();
    }

    void Update()
    {
        if (PlayerInRange())
        {
            RotateTurret();
            if (!isFiring)
            {
                StartCoroutine(FireRoutine());
            }
        }
    }

    void RotateTurret()
    {
        Transform closestPlayer = player.transform;
        aimHolder.LookAt(closestPlayer);
        Vector3 targetDir = closestPlayer.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
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
        _bullet.GetComponent<Bullet>().InitializeBullet(bulletDamage, bulletSpeed, bulletLifetime);

        if (muzzleFlashVfx != null)
        {
            muzzleFlashVfx.Play();
        }
    }

    bool PlayerInRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < playerRange)
        {
          return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerRange);
    }
}
