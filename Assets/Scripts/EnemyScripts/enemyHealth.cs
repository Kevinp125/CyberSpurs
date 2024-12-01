using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private enemyStats enemyStats;
    [SerializeField] private Slider healthBarSlider;
    public PlayerBulletTime playerBulletTime; //getting a reference to our BulletTime script so we can call our OnEnemyKilled() fucnction
    public PlayerStats playerStats; // Reference to PlayerStats script for tracking kills

    public float currentHealth;

    private void Start()
    {
        currentHealth = enemyStats.maxHealth;
        SetHealhBarUI();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerBulletTime = player.GetComponent<PlayerBulletTime>();
            playerStats = player.GetComponent<PlayerStats>();
        }

        // Log errors if references are not found
        if (playerBulletTime == null)
        {
            Debug.LogError("PlayerBulletTime script not found on the Player! Ensure it's attached.");
        }

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats script not found on the Player! Ensure it's attached.");
        }
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        CheckIfDead();
        SetHealhBarUI();
    }

    private void CheckIfDead()
    {
        if (currentHealth <= 0)
        {
            if (playerStats != null)
            {
                playerStats.AddKill();
            }
            else
            {
                Debug.LogError("playerStats is null! Ensure it's assigned.");
            }

            if (playerBulletTime != null)
            {
                playerBulletTime.OnEnemyKilled();
            }
            else
            {
                Debug.LogError("playerBulletTime is null! Ensure it's assigned.");
            }

            Destroy(gameObject);
        }
    }

    private void SetHealhBarUI()
    {
        healthBarSlider.value = CalculateHealthPercentage();
    }

    private float CalculateHealthPercentage()
    {
        return (currentHealth / enemyStats.maxHealth) * 100;
    }
}
