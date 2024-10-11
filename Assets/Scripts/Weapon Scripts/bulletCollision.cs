using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletCollision : MonoBehaviour
{
    public float stickDuration = 2f; //duration that our bullet is going to stick to a wall once it collides to it
  // This function is called automatically by Unity when the bullet collides with another object
    private void OnCollisionEnter(Collision collision)
    {
       // Check if the bullet hit a wall (you can use tags or layers to limit this to specific objects)
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Stop the bullet's movement
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;  // Stop the bullet
                rb.isKinematic = true;       // Disable physics on the bullet to make it stick
            }

            // Parent the bullet to the wall so it stays attached even if the wall moves
            transform.SetParent(collision.transform);

            // Optionally, change the bullet's position slightly to adjust its sticking point (if needed)

            // Start the coroutine to destroy the bullet after a delay
            StartCoroutine(DestroyBulletAfterDelay(stickDuration));
        }

         else if (collision.gameObject.CompareTag("Target"))
        {
            // Destroy the bullet immediately when it hits a target
            Destroy(gameObject);
            
            // Optionally, apply any target-specific behavior here, like reducing the target's health
            // Target target = collision.gameObject.GetComponent<Target>();
            // if (target != null) target.TakeDamage(damageAmount);
        }
        
        else
        {
            // If it hits anything else, destroy the bullet immediately
            Destroy(gameObject);
        }
    }

    // Coroutine that waits for a few seconds before destroying the bullet
    private IEnumerator DestroyBulletAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified duration
        Destroy(gameObject);                    // Destroy the bullet
    }

}

