using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol: MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> waypoints; // List of patrol waypoints
    [SerializeField] private float waitTime = 2f; // Time to wait at each waypoint
    [SerializeField] private float stoppingDistance = 0.2f; // How close the agent needs to be to consider it reached the waypoint

    [Header("Chase Settings")]
    [SerializeField] private Transform player; // Reference to the player's Transform
    [SerializeField] private float detectionRange = 10f; // Range within which the enemy will detect and chase the player
    [SerializeField] private float chaseStoppingDistance = 2f; // Stopping distance when chasing the player

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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position; // Adjust the position to snap to NavMesh
        }

        if (distanceToPlayer <= detectionRange)
        {
            // Switch to chasing when the player is close
            currentState = EnemyState.Chasing;
        }
        else
        {
            // Switch back to patrolling when the player is out of range
            currentState = EnemyState.Patrolling;
        }

        if (currentState == EnemyState.Patrolling)
        {
            Patrol();
        }
        else if (currentState == EnemyState.Chasing)
        {
            ChasePlayer();
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

    private void ChasePlayer()
    {
        agent.stoppingDistance = chaseStoppingDistance; // Set stopping distance for chasing
        agent.SetDestination(player.position); // Chase the player's position
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection range in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
