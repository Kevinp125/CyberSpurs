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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position; // Adjust the position to snap to NavMesh
        }

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
            currentState = EnemyState.Patrolling;
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

        if (agent.isOnNavMesh)
        {
            Debug.Log("Agent is on the NavMesh.");
        }
        else
        {
            Debug.LogError("Agent is NOT on the NavMesh.");
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
        // Find all colliders within the aggro radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aggroRadius);

        foreach (Collider nearbyCollider in hitColliders)
        {
            // Check for EnemyPatrol
            EnemyPatrolChaseShoot nearbyEnemy = nearbyCollider.GetComponent<EnemyPatrolChaseShoot>();
            if (nearbyEnemy != null && nearbyEnemy != this)
            {
                Debug.Log($"{nearbyEnemy.gameObject.name} (EnemyPatrol) is now chasing the player because of aggro!");
                nearbyEnemy.ChasePlayer(); // Trigger the chase behavior for EnemyPatrol
                continue; // Skip to the next collider to avoid duplicate checks
            }

            // Check for EnemyPatrolChaseScript
            EnemyPatrol nearbyChaseEnemy = nearbyCollider.GetComponent<EnemyPatrol>();
            if (nearbyChaseEnemy != null)
            {
                Debug.Log($"{nearbyChaseEnemy.gameObject.name} (EnemyPatrolChaseScript) is now chasing the player because of aggro!");
                nearbyChaseEnemy.ChasePlayer(); // Trigger the chase behavior for EnemyPatrolChaseScript
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
        agent.stoppingDistance = shootRange; // Keep the enemy at a shooting distance
        agent.SetDestination(player.position);

        // Rotate toward the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Handle shooting cooldown
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
            enemyGunAudio.Play();     // Play the enemy shoot sound effect
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        }
        else
        {
            Debug.LogWarning("ProjectilePrefab or ShootPoint is not assigned.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection and shooting ranges in the Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
