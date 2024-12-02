using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private enemyStats enemyStats;
    [SerializeField] private Slider healthBarSlider;
    public PlayerBulletTime playerBulletTime; // Reference to BulletTime script
    public PlayerStats playerStats; // Reference to PlayerStats script

    public float currentHealth;
    public float separationDistance = 2f; // Minimum distance to maintain from other enemies
    public float separationStrength = 5f; // Speed of the separation movement

    private UnityEngine.AI.NavMeshAgent agent;

    private void Start()
    {
        currentHealth = enemyStats.maxHealth;
        SetHealhBarUI();

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found! Ensure this enemy has a NavMeshAgent component.");
            return;
        }

        // Set a randomized avoidance priority
        agent.avoidancePriority = Random.Range(30, 60);

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

    private void Update()
    {
        ApplySeparation();
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

    private void ApplySeparation()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, separationDistance);
        Vector3 separationForce = Vector3.zero;

        foreach (Collider collider in nearbyEnemies)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Target"))
            {
                Vector3 directionAway = transform.position - collider.transform.position;
                float distance = directionAway.magnitude;

                if (distance < separationDistance && distance > 0.1f) // Avoid dividing by zero
                {
                    separationForce += directionAway.normalized / distance; // Stronger repulsion for closer enemies
                }
            }
        }

        // Apply separation as a small influence to the agent's velocity
        if (separationForce != Vector3.zero)
        {
            separationForce = separationForce.normalized * 0.5f; // Scale down the force
            agent.velocity += separationForce * Time.deltaTime; // Blend with existing velocity
        }
    }
}
