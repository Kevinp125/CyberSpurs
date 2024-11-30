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
    private bool isVulnerable = false; // Whether the boss can be damaged
    [SerializeField] private Shotgun shotgun;

    [Header("Bullet Time Settings")]
    [SerializeField] private float bulletTimeIncreasePercent = 10f;

    [Header("Phase Two Settings")]
    [SerializeField] private GameObject[] guardDogs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float vulnerabilityDuration = 5f; // Time boss is vulnerable
    [SerializeField] private float respawnDelay = 3f; // Time before respawning guard dogs

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Array of predefined patrol points
    private int currentPatrolIndex = 0; // Index of the current patrol point

    [Header("NavMesh Settings")]
    private NavMeshAgent navAgent;
    private Transform player; // Reference to the player
    [SerializeField] public float sightRange = 15f; // How far the boss can see the player
    [SerializeField] public float attackRange = 5f; // Range at which the boss attacks

    private List<GameObject> activeGuardDogs = new List<GameObject>();
    private BossState currentState = BossState.Patrolling;
    public GameManager gameManager; // Reference to the GameManager

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
                PhaseTwoBehavior();
                break;
        }
    }

    private void PhaseTwoBehavior()
    {
        // Keep chasing and attacking in Phase Two
        ChasePlayer();
        AttackPlayer();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned!");
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        navAgent.SetDestination(targetPoint.position);

        if (Vector3.Distance(transform.position, targetPoint.position) < 1f)
        {
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
        if (playerBulletTime.isPlayerInShadows)
        {
            shotgun.spreadAngle = 60f;
            Debug.Log("Player is in the shadows! Boss fires blindly.");
        }
        else
        {
            shotgun.spreadAngle = 30f;
            Debug.Log("Player is not in the shadows! Boss fires accurately.");
        }

        shotgun.Fire();
    }

    private bool PlayerInSight()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        if (Vector3.Distance(transform.position, player.position) <= sightRange &&
            Vector3.Angle(transform.forward, directionToPlayer) < 45f)
        {
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
        if (!isPhaseTwo || isVulnerable)
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
        else
        {
            Debug.Log("Boss is invulnerable while guard dogs are active!");
        }
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

        SpawnGuardDogs();
    }

    private void SpawnGuardDogs()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            GameObject guardDog = Instantiate(
                guardDogs[Random.Range(0, guardDogs.Length)],
                spawnPoint.position,
                Quaternion.identity
            );

            activeGuardDogs.Add(guardDog);
        }
    }

    public void OnGuardDogDestroyed(GameObject guardDog)
    {
        activeGuardDogs.Remove(guardDog);

        if (activeGuardDogs.Count == 0)
        {
            StartCoroutine(VulnerabilityWindow());
        }
    }

    private IEnumerator VulnerabilityWindow()
    {
        Debug.Log("Boss is vulnerable!");
        isVulnerable = true;
        yield return new WaitForSeconds(vulnerabilityDuration);

        Debug.Log("Boss is no longer vulnerable. Respawning guard dogs...");
        isVulnerable = false;
        yield return new WaitForSeconds(respawnDelay);

        SpawnGuardDogs();
    }

    private void CheckIfDead()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            if (gameManager != null)
            {
                gameManager.DisplayEndStats();
            }
            else
            {
                Debug.LogError("GameManager reference is missing in BossHealth!");
            }

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
