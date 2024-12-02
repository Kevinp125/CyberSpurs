using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float stoppingDistance = 0.2f;

    [Header("Chase Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float chaseStoppingDistance = 2f;

    [Header("Aggro Settings")]
    [SerializeField] private float aggroRadius = 10f;
    private bool hasAggroed = false;

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0;
    private Transform player;

    private enum EnemyState { Patrolling, Chasing }
    private EnemyState currentState = EnemyState.Patrolling;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent.enabled) agent.enabled = true;

        FindPlayer();

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
        if (player == null)
        {
            FindPlayer();
            return; // Skip the rest of the update if the player is not found yet
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Determine the current state based on player's proximity
        if (distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chasing;
            hasAggroed = true; // Aggro is active
        }
        else
        {
            currentState = EnemyState.Patrolling;
            hasAggroed = false; // Reset aggro when player is out of range
        }

        // Execute behavior based on the current state
        if (currentState == EnemyState.Patrolling)
        {
            Patrol();
        }
        else if (currentState == EnemyState.Chasing)
        {
            ChasePlayer();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Ensure the player has the 'Player' tag.");
        }
    }

    private void Patrol()
    {
        if (waypoints.Count == 0) return;

        agent.stoppingDistance = stoppingDistance; // Reset stopping distance for patrol
        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                currentWaypointIndex = Random.Range(0, waypoints.Count);

                agent.SetDestination(waypoints[currentWaypointIndex].position);
                waitTimer = 0;
            }
        }
    }

    public void ChasePlayer()
    {
        if (player != null)
        {
            agent.stoppingDistance = chaseStoppingDistance; // Adjust stopping distance for chasing
            agent.SetDestination(player.position);
        }
    }

    public void TakeDamage()
    {
        if (!hasAggroed)
        {
            Debug.Log($"{gameObject.name} has been shot and is now chasing the player!");
            ChasePlayer();
            AggroNearbyEnemies();
            hasAggroed = true;
        }
    }

    private void AggroNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aggroRadius);
        foreach (Collider nearbyCollider in hitColliders)
        {
            EnemyPatrol nearbyEnemy = nearbyCollider.GetComponent<EnemyPatrol>();
            if (nearbyEnemy != null && nearbyEnemy != this)
            {
                nearbyEnemy.ChasePlayer();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
}
