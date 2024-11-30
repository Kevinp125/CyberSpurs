using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> waypoints; // List of patrol waypoints
    [SerializeField] private float waitTime = 2f; // Time to wait at each waypoint
    [SerializeField] private float stoppingDistance = 0.2f; // How close the agent needs to be to consider it reached the waypoint

    [Header("Chase Settings")]
    [SerializeField] private Transform player; // Reference to the player's Transform
    [SerializeField] private float detectionRange = 10f; // Range within which the enemy will detect and chase the player
    [SerializeField] private float chaseStoppingDistance = 2f; // Stopping distance when chasing the player

    [Header("Aggro Settings")]
    [SerializeField] private float aggroRadius = 10f; // Radius to find nearby enemies when aggroed
    private bool hasAggroed = false; // Ensure aggro only triggers once per damage event

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0;

    private enum EnemyState { Patrolling, Chasing }
    private EnemyState currentState = EnemyState.Patrolling;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        if (waypoints.Count > 0)
        {
            // Set initial patrol destination
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        else
        {
            Debug.LogWarning("No waypoints set for patrol.");
        }
    }

    void Update()
    {
        if (currentState == EnemyState.Patrolling)
        {
            Patrol();
        }
        else if (currentState == EnemyState.Chasing)
        {
            ChasePlayer();
        }
    }

    private void Patrol()
    {
        if (waypoints.Count == 0) return;

        // Check if the agent is close enough to the current waypoint
        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            // If the agent has waited long enough, move to the next waypoint
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
        currentState = EnemyState.Chasing; // Set the state to chasing
        agent.stoppingDistance = chaseStoppingDistance; // Set stopping distance for chasing
        agent.SetDestination(player.position); // Chase the player's position
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
            EnemyPatrol nearbyEnemy = nearbyCollider.GetComponent<EnemyPatrol>();
            if (nearbyEnemy != null && nearbyEnemy != this)
            {
                Debug.Log($"{nearbyEnemy.gameObject.name} (EnemyPatrol) is now chasing the player because of aggro!");
                nearbyEnemy.ChasePlayer(); // Trigger the chase behavior for EnemyPatrol
                continue; // Skip to the next collider to avoid duplicate checks
            }

            // Check for EnemyPatrolChaseScript
            EnemyPatrolChaseShoot nearbyChaseEnemy = nearbyCollider.GetComponent<EnemyPatrolChaseShoot>();
            if (nearbyChaseEnemy != null)
            {
                Debug.Log($"{nearbyChaseEnemy.gameObject.name} (EnemyPatrolChaseScript) is now chasing the player because of aggro!");
                nearbyChaseEnemy.ChasePlayer(); // Trigger the chase behavior for EnemyPatrolChaseScript
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection range in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Visualize the aggro radius in the Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
}
