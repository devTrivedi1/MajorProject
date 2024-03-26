using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [HideInInspector] public Movement player;

    [HorizontalLine("Projectile Settings ", 2, FixedColor.CherryRed)]
    public int projectileSpeed = 5;
    public float projectileLifetime = 5f;
    public int projectileDamage = 1;

    [HorizontalLine("Turret Activation Settings ", 2, FixedColor.CherryRed)]
    public Color gizmoColor = Color.red;
    public float rotationSpeed = 5f;
    public float fireRate = 1f;
    public float playerRange = 10f; // Range within which players are detected

    [HorizontalLine("References ", 2, FixedColor.CherryRed)]
    public GameObject bulletPrefab;
    public Transform aimHolder;
    public Transform firePoint;


    void Start()
    {
        player = FindObjectOfType<Movement>();
    }

    public void Rotate()
    {
        Transform closestPlayer = player.transform;
        if (aimHolder != null) { aimHolder.LookAt(closestPlayer); }
        Vector3 targetDir = closestPlayer.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public bool PlayerInRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < playerRange)
        {
            return true;
        }
        return false;
    }
}