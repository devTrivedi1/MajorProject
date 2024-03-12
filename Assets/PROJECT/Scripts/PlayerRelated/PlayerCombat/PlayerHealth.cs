using System;
using UnityEngine;

public class PlayerHealth : ObjectHealth
{
    public static Action OnPlayerDeath;
    public static Action<int,GameObject> OnPlayerHealthInitialized;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Start()
    {
        OnPlayerHealthInitialized?.Invoke(MaxHealth, gameObject);
    }
    protected override void ObjectDeath()
    {
        OnPlayerDeath?.Invoke();
    }
}
