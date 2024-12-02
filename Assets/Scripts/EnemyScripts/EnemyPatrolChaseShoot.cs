using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolChaseShoot : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> waypoints; // List of patrol waypoints
    [SerializeField] private float waitTime = 2f; // Time to wait at each waypoint
    [SerializeField] private float patrolStoppingDistance = 0.2f; // Stopping distance for patrol

    [Header("Chase Settings")]
    [SerializeField] private Transform player; // Reference to the player's Transform
    [SerializeField] private float detectionRange = 15f; // Range within which the enemy will detect and chase the player
    [SerializeField] private float chaseStoppingDistance = 2f; // Stopping distance when chasing the player

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab; // Projectile to shoot at the player
    [SerializeField] private Transform shootPoint; // Point from which the projectile will be shot
    [SerializeField] private float shootRange = 8f; // Range within which the enemy can shoot the player
    [SerializeField] private float fireRate = 1f; // Time between shots

    [Header("Aggro Settings")]
    [SerializeField] private float aggroRadius = 10f; // Radius to find nearby enemies when aggroed
    private bool hasAggroed = false; // Ensure aggro only triggers once per damage event

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0;
    private float shootTimer = 0;

    [Header("Animation Settings")]
    [SerializeField] private Animator enemyAnimator; // Reference to the Animator
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private enum EnemyState { Patrolling, Chasing, Shooting }
    private EnemyState currentState = EnemyState.Patrolling;

    public AudioSource enemyGunAudio;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
        }

        if (waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        else
        {
            Debug.LogWarning("No waypoints set for patrol.");
        }
    }

    void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent is NOT on the NavMesh.");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Determine state based on player's distance
        if (distanceToPlayer <= shootRange)
        {
            currentState = EnemyState.Shooting;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            // Return to patrolling when the player is out of detection range
            if (currentState != EnemyState.Patrolling)
            {
                Debug.Log($"{gameObject.name} is returning to patrol.");
                hasAggroed = false; // Reset aggro state
                currentState = EnemyState.Patrolling;
                ResetAgentForPatrol();
            }
        }

        // Execute behavior based on the current state
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Shooting:
                ShootPlayer();
                break;
        }

        UpdateAnimation(); // Update animation based on movement
    }

    private void UpdateAnimation()
    {
        if (enemyAnimator != null)
        {
            bool isWalking = agent.velocity.magnitude > 0.1f; // Check if the enemy is moving
            enemyAnimator.SetBool(IsWalking, isWalking);
        }
    }

    private void ResetAgentForPatrol()
    {
        agent.stoppingDistance = patrolStoppingDistance;
        if (waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    public void TakeDamage()
    {
        // Aggro the enemy that was shot
        if (!hasAggroed)
        {
            Debug.Log($"{gameObject.name} has been shot and is now chasing the player!");
            ChasePlayer();
            AggroNearbyEnemies(); // Notify nearby enemies to aggro
            hasAggroed = true; // Ensure aggro happens only once
        }
    }

    private void AggroNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aggroRadius);

        foreach (Collider nearbyCollider in hitColliders)
        {
            EnemyPatrolChaseShoot nearbyEnemy = nearbyCollider.GetComponent<EnemyPatrolChaseShoot>();
            if (nearbyEnemy != null && nearbyEnemy != this)
            {
                Debug.Log($"{nearbyEnemy.gameObject.name} is now chasing the player due to aggro.");
                nearbyEnemy.ChasePlayer();
                continue;
            }
        }
    }

    private void Patrol()
    {
        if (waypoints.Count == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= patrolStoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
                waitTimer = 0;
            }
        }
    }

    public void ChasePlayer()
    {
        agent.stoppingDistance = chaseStoppingDistance;
        agent.SetDestination(player.position);
    }

    private void ShootPlayer()
    {
        agent.stoppingDistance = shootRange;
        agent.SetDestination(player.position);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        shootTimer += Time.deltaTime;
        if (shootTimer >= fireRate)
        {
            FireProjectile();
            shootTimer = 0;
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            enemyGunAudio.Play();
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        }
        else
        {
            Debug.LogWarning("ProjectilePrefab or ShootPoint is not assigned.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
