using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f; // Speed of the projectile
     public float stickDuration = 2f; // Time the bullet sticks to the wall before being destroyed
    public float lifetime = 5f; // Time before the projectile is destroyed

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Apply initial velocity in the forward direction
        rb.velocity = transform.forward * speed;

        // Destroy the projectile after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthManager.Damage(10);
            Debug.Log("Player hit!");

            // Optionally destroy the projectile
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            
            Debug.Log("Hit the environment!");
            StickToWall(collision.contacts[0].point, collision.contacts[0].normal);
            // Destroy the projectile
            Destroy(gameObject, stickDuration);
        }
    }

    private void StickToWall(Vector3 collisionPoint, Vector3 collisionNormal)
    {
        // Stop movement by disabling the Rigidbody
        rb.isKinematic = true;

        // Position the projectile at the collision point
        transform.position = collisionPoint;

        // Align the projectile to the wall's surface
        transform.rotation = Quaternion.LookRotation(collisionNormal);
    }
}
