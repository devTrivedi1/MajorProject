
using UnityEngine;
using CustomInspector;

public class ObjectHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int MaxHealth;
    [ReadOnly][SerializeField] int currentHealth;

    protected virtual void OnEnable()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int Amount)
    {
        currentHealth -= Amount;

        IDamageable.OnDamageTaken?.Invoke(Amount, gameObject);

        if (currentHealth <= 0)
        {
            ObjectDeath();
        }
    }

    protected virtual void ObjectDeath() => Debug.Log("object is dead");
}
