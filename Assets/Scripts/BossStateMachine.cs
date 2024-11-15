using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BossHealth : MonoBehaviour, IDamageable
{
    public enum BossState
    {
        Patrolling,
        Chasing,
        Attacking,
        PhaseTwo
    }

    [Header("Boss Stats")]
    [SerializeField] private enemyStats enemyStats;
    [SerializeField] private Slider healthBarSlider;
    public PlayerBulletTime playerBulletTime;
    public float currentHealth;
    private bool isPhaseTwo;

    [Header("Bullet Time Settings")]
    [SerializeField] private float bulletTimeIncreasePercent = 10f;

    [Header("Phase Two Settings")]
    [SerializeField] private GameObject[] guardDogs;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Array of predefined patrol points
    private int currentPatrolIndex = 0; // Index of the current patrol point


    [Header("NavMesh Settings")]
    private NavMeshAgent navAgent;
    private Transform player; // Reference to the player
    public float sightRange = 15f; // How far the boss can see the player
    public float attackRange = 5f; // Range at which the boss attacks

    private BossState currentState = BossState.Patrolling;

    private void Start()
    {
        currentHealth = enemyStats.maxHealth;
        SetHealthBarUI();

        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform; // Assuming player has a "Player" tag
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Patrolling:
                Patrol();
                if (PlayerInSight())
                {
                    currentState = BossState.Chasing;
                }
                break;

            case BossState.Chasing:
                ChasePlayer();
                if (Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    currentState = BossState.Attacking;
                }
                else if (!PlayerInSight())
                {
                    currentState = BossState.Patrolling;
                }
                break;

            case BossState.Attacking:
                AttackPlayer();
                if (Vector3.Distance(transform.position, player.position) > attackRange)
                {
                    currentState = BossState.Chasing;
                }
                break;

            case BossState.PhaseTwo:
                // Phase Two-specific behavior
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned!");
            return;
        }

        // Get the current patrol point
        Transform targetPoint = patrolPoints[currentPatrolIndex];

        // Set the destination for the NavMeshAgent
        navAgent.SetDestination(targetPoint.position);

        // Check if the boss has reached the patrol point
        if (Vector3.Distance(transform.position, targetPoint.position) < 1f)
        {
           // Move to the next patrol point (loop back to the first point if at the end)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            navAgent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        // Example attack logic
        Debug.Log("Boss is attacking!");
        // Add attack animations, projectiles, etc., here
    }

    private bool PlayerInSight()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        if (Vector3.Distance(transform.position, player.position) <= sightRange &&
            Vector3.Angle(transform.forward, directionToPlayer) < 45f)
        {
            // Optionally add raycast for line of sight
            Ray ray = new Ray(transform.position, directionToPlayer);
            if (Physics.Raycast(ray, out RaycastHit hit, sightRange))
            {
                return hit.transform.CompareTag("Player");
            }
        }
        return false;
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;

        if (playerBulletTime != null)
        {
            playerBulletTime.IncreaseBulletTime(bulletTimeIncreasePercent);
        }

        SetHealthBarUI();
        CheckIfPhaseTwo();
        CheckIfDead();
    }

    private void CheckIfPhaseTwo()
    {
        if (!isPhaseTwo && currentHealth <= enemyStats.maxHealth / 2)
        {
            EnterPhaseTwo();
        }
    }

    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;
        currentState = BossState.PhaseTwo;

        // Stop patrolling or chasing
        if (navAgent != null) navAgent.ResetPath();

        foreach (var spawnPoint in spawnPoints)
        {
            Instantiate(guardDogs[Random.Range(0, guardDogs.Length)], spawnPoint.position, Quaternion.identity);
        }
    }

    private void CheckIfDead()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void SetHealthBarUI()
    {
        healthBarSlider.value = CalculateHealthPercentage();
    }

    private float CalculateHealthPercentage()
    {
        return (currentHealth / enemyStats.maxHealth) * 100;
    }
}
