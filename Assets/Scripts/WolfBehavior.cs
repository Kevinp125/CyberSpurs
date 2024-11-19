using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GuardDog : MonoBehaviour, IDamageable
{
    [Header("Guard Dog Settings")]
    [SerializeField] private enemyStats enemyStats; // Guard dog health
    [SerializeField] private int damage = 10; // Damage dealt to player on contact
    [SerializeField] private Slider healthBarSlider; // Health bar slider for the guard dog
    [SerializeField] private float collisionRadius = 1f; // Radius to detect the player
    [SerializeField] private float pushRadius = 1f; // Radius to detect nearby wolves
    [SerializeField] private float pushStrength = 0.5f; // Strength of the push force
    [SerializeField] private float damageCooldown = 2f; // Cooldown time after damaging the player
    public PlayerBulletTime playerBulletTime; // Cooldown time after damaging the player
    [SerializeField] private float bulletTimeIncrease; // Cooldown time after damaging the player

    private float currentHealth;
    private NavMeshAgent navAgent;
    private Transform player;
    private bool canDamage = true; // Whether the wolf can currently damage the player

    private void Start()
    {
        // Initialize NavMeshAgent and find the player
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform; // Assuming the player has the "Player" tag
        currentHealth = enemyStats.maxHealth;
        playerBulletTime = FindObjectOfType<PlayerBulletTime>();
        // Initialize the health bar slider if assigned
        SetHealthBarUI();
    }

    private void Update()
    {
        if (player == null) return;

        // Chase the player if allowed
        if (canDamage)
        {
            navAgent.SetDestination(player.position);
        }

        // Check for collisions with the player
        DetectPlayerCollision();

        // Push away from other wolves
        SeparateFromOtherWolves();
    }

    private void DetectPlayerCollision()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, collisionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player") && canDamage)
            {
                // Deal damage to the player
                HealthManager.Damage(damage);
                Debug.Log("Guard Dog damaged the player!");

                // Trigger cooldown to stop chasing
                StartCoroutine(DamageCooldown());
                return; // Prevent multiple damage calls in one frame
            }
        }
    }

    private void SeparateFromOtherWolves()
    {
        Collider[] nearbyWolves = Physics.OverlapSphere(transform.position, pushRadius);
        foreach (Collider wolf in nearbyWolves)
        {
            if (wolf.gameObject != this.gameObject && wolf.CompareTag("GuardDog"))
            {
                Vector3 pushDirection = transform.position - wolf.transform.position;
                navAgent.Move(pushDirection.normalized * pushStrength * Time.deltaTime);
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        navAgent.ResetPath(); // Stop chasing during cooldown
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Guard Dog took {damageAmount} damage! Remaining health: {currentHealth}");

        // Update health bar slider
        SetHealthBarUI();

        // Check if the guard dog is dead
        if (currentHealth <= 0)
        {
            // Notify the boss that this guard dog is destroyed
            BossHealth boss = FindObjectOfType<BossHealth>();
            if (boss != null)
            {
                boss.OnGuardDogDestroyed(gameObject);
            }

            playerBulletTime.IncreaseBulletTime(bulletTimeIncrease);
            // Destroy this guard dog
            Destroy(gameObject);
        }
    }

    private void SetHealthBarUI()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = CalculateHealthPercentage();
        }
        else
        {
            Debug.LogWarning("Health bar slider is not assigned to GuardDog.");
        }
    }

    private float CalculateHealthPercentage()
    {
        return (currentHealth / enemyStats.maxHealth) * 100;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the collision detection radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collisionRadius);

        // Visualize the push radius in the editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pushRadius);
    }
}
