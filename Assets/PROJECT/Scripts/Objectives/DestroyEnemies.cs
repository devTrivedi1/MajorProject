public class DestroyEnemies : Objective
{
    EnemyBase[] enemies;

    private void Start()
    {
        enemies = FindObjectsOfType<EnemyBase>();
    }

    public override bool CheckCompletion()
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