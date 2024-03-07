using UnityEngine;

public interface IDamagable
{
    public GameObject gameObject {  get; }
    public Transform transform { get; }

    public void TakeDamage();
}