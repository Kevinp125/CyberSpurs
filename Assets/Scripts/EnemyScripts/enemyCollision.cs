using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCollision : MonoBehaviour
{
    public float collisionRadius;

    public int damageAmount;
    public PlayerStats playerStats;
    // Start is called before the first frame update
    void Start()
    {
            playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found in the scene! Ensure the Player GameObject has the PlayerStats script.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, collisionRadius);

        foreach(Collider nearbyObject in colliders)
        {
            if(nearbyObject.tag == "Player")
            {
                HealthManager.Damage(damageAmount);

                    if (playerStats != null)
                    {
                        playerStats.TakeDamage(); // Call TakeDamage on the player
                    }
                    else
                    {
                        Debug.LogError("PlayerStats is null! Ensure it's assigned.");
                    }
            }
        }

    }
}
