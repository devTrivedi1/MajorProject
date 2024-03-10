using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    Movement player;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float playerRange = 10f; // Range within which players are detected
    [SerializeField] float timeBetweenBullets;
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
        firePoint.transform.LookAt(closestPlayer);
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
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
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
