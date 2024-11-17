using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardDog : MonoBehaviour, IDamageable
{
    [Header("Guard Dog Settings")]
    [SerializeField] private float health = 50f; // Guard dog health
    [SerializeField] private int damage = 10; // Damage dealt to player on contact
    [SerializeField] private float chaseRange = 15f; // How far the guard dog can detect the player

    private NavMeshAgent navAgent;
    private Transform player;

    private void Start()
    {
        // Initialize NavMeshAgent and find the player
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform; // Assuming the player has the "Player" tag
    }

    private void Update()
    {
        if (player == null) return;

        // Chase the player if within range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRange)
        {
            navAgent.SetDestination(player.position);
        }
        else
        {
            navAgent.ResetPath(); // Stop moving if the player is out of range
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player
            HealthManager.Damage(damage);
            Debug.Log("Guard Dog damaged the player!");

        }
    }

    public void Damage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log($"Guard Dog took {damageAmount} damage! Remaining health: {health}");

        if (health <= 0)
        {
            // Notify the boss that this guard dog is destroyed
            BossHealth boss = FindObjectOfType<BossHealth>();
            if (boss != null)
            {
                boss.OnGuardDogDestroyed(gameObject);
            }

            // Destroy this guard dog
            Destroy(gameObject);
        }
    }
}
