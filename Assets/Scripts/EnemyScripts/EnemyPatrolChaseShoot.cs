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

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0;
    private float shootTimer = 0;

    private enum EnemyState { Patrolling, Chasing, Shooting }
    private EnemyState currentState = EnemyState.Patrolling;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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

    private void ChasePlayer()
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
