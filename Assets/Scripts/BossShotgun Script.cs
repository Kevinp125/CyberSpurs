using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [Header("Shotgun Settings")]
    [SerializeField] private GameObject projectilePrefab; // Prefab for the shotgun projectiles
    [SerializeField] private Transform firePoint; // Fire point for projectiles
    [SerializeField] private int pelletCount = 5; // Number of pellets per shot
    [SerializeField] public float spreadAngle = 30f; // Spread angle of the pellets
    [SerializeField] private float fireRate = 1f; // Time between shots

    private float lastFireTime; // Tracks the last time the shotgun fired

    public void Fire()
    {
        if (Time.time - lastFireTime < 1f / fireRate)
        {
            return; // Prevent firing too quickly
        }

        lastFireTime = Time.time;

        // Spawn pellets
        for (int i = 0; i < pelletCount; i++)
        {
            // Calculate spread
            Vector3 spreadDirection = Quaternion.Euler(
                0,
                Random.Range(-spreadAngle / 2, spreadAngle / 2),
                0
            ) * firePoint.forward;
            
            if (projectilePrefab == null || firePoint == null)
            {
                Debug.LogError("Projectile Prefab or Fire Point is not assigned!");
                return;
            }
            
            // Instantiate projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(spreadDirection));
            Debug.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 5, Color.green, 2f);
            if (projectile != null)
            {
                Debug.Log($"Projectile {i + 1} instantiated successfully at {firePoint.position}.");
            }
            else
            {
                Debug.LogError($"Failed to instantiate projectile {i + 1}.");
            }

        }
    }
}
