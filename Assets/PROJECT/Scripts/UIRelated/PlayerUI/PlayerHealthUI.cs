using CustomInspector;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class PlayerHealthUI : UiGridSpawner
{
    Movement player;

    [HorizontalLine("Player Health Ui Settings", 2, FixedColor.CherryRed)]
    [Tooltip("Represents the health amount  of one heart\r\n")]
    [SerializeField] int heartUIHealthValue;

    [SerializeField][ReadOnly]int healthLeftInCurrentHeart;

    private void OnEnable()
    {
        IDamageable.OnDamageTaken += UpdateHealthUI;
        PlayerHealth.OnPlayerHealthInitialized += InitializePlayerHealthUI;

        player = FindObjectOfType<Movement>();
        healthLeftInCurrentHeart = heartUIHealthValue;
    }
    private void OnDisable()
    {
        IDamageable.OnDamageTaken -= UpdateHealthUI;
        PlayerHealth.OnPlayerHealthInitialized -= InitializePlayerHealthUI;
    }
    void InitializePlayerHealthUI(int _maxHealth, GameObject _player)
    {
        if (_player != player.gameObject) return;
        if (heartUIHealthValue == 0) return;

        int amountOfHeartsToSpawn = _maxHealth / heartUIHealthValue;
        SpawnUiInBulk(uiPrefabToSpawn, amountOfHeartsToSpawn);
    }

    void UpdateHealthUI(int damageTaken, GameObject _player)
    {
        if (_player != player.gameObject) return;
        if (spawnedUI.Count == 0) { return; }

        healthLeftInCurrentHeart -= damageTaken;
        if (healthLeftInCurrentHeart <= 0)
        {
            healthLeftInCurrentHeart = heartUIHealthValue;
            Destroy(spawnedUI.Last());
            spawnedUI.RemoveAt(spawnedUI.Count - 1);
        }
    }
}