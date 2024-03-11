using System;

public class PlayerHealth : ObjectHealth
{
    public static Action OnPlayerDeath;

    protected override void ObjectDeath()
    {
        OnPlayerDeath?.Invoke();
    }
}
