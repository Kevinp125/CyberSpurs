using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private enemyStats enemyStats;
    [SerializeField] private Slider healthBarSlider;
    public PlayerBulletTime playerBulletTime; //getting a reference to our BulletTime script so we can call our OnEnemyKilled() fucnction

    public float currentHealth;

    private void Start()
    {
        currentHealth = enemyStats.maxHealth;
        SetHealhBarUI();
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
