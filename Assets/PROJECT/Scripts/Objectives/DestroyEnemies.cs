using CustomInspector;
using UnityEngine;

public class DestroyEnemies : Objective
{
    [SerializeField, ReadOnly] EnemyBase[] enemies;

    private void Start()
    {
        enemies = FindObjectsOfType<EnemyBase>();
    }

    protected override bool CheckCompletion()
    {
        bool allDead = true;
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeSelf)
            {
                allDead = false;
                break;
            }
        }
        return allDead;
    }
}