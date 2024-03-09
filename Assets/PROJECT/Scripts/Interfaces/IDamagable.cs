using UnityEngine;

public interface IDamageable
{
    public GameObject gameObject {  get; }
    public Transform transform { get; }

    public void TakeDamage(int damage);
}