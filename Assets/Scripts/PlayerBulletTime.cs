using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBulletTime : MonoBehaviour
{
    // Variables for bullet time
    public float bulletTimeMeter = 0f;  // Current bullet time meter value
    public float maxBulletTimeMeter = 100f;  // Max value for the bullet time meter
    public float bulletTimeDuration = 5f;  // Duration of bullet time (in seconds)
    private bool isBulletTimeActive = false;  // Is bullet time currently active?
    private bool isInShadows = false;  // Tracks if the player is in the shadows
    [SerializeField] private KeyCode activationKey = KeyCode.B;
    [SerializeField] private Transform takedownCheck;  // Position to raycast from (e.g., player)
    [SerializeField] private LayerMask enemyLayer;  // LayerMask for enemies
    [SerializeField] private LayerMask shadowLayer;  // LayerMask for shadows
    [SerializeField] private float bulletTimeScale = 0.2f;  // LayerMask for shadows
    [SerializeField] private float bulletTimeMeterIncrease = 20f;

    // UI elements (optional)
    public Image bulletTimeBar;  // Progress bar for the bullet time meter

    void Update()
    {
        // Check if the player presses "B" and the bullet time meter is full
        if (Input.GetKeyDown(activationKey) && bulletTimeMeter >= maxBulletTimeMeter && !isBulletTimeActive)
        {
            ActivateBulletTime();  // Activate bullet time when the meter is full
        }

        // Update the UI for the bullet time meter (optional)
        if (bulletTimeBar != null)
        {
            bulletTimeBar.fillAmount = bulletTimeMeter / maxBulletTimeMeter;  // Update the progress bar
        }
    }

    // Call this function when an enemy is killed outside the shadows
    public void OnEnemyKilled()
    {
        if (CheckEnemyAndPlayerInShadow())
        {
            // Do nothing or handle shadow case (optional)
            Debug.Log("Enemy is in the shadow, no bullet time increase.");
        }
        else
        {
            // Increase bullet time meter if enemy is outside the shadows
            bulletTimeMeter += bulletTimeMeterIncrease;  // Increase the meter by 20 per kill (adjust as needed)
            if (bulletTimeMeter > maxBulletTimeMeter)
            {
                bulletTimeMeter = maxBulletTimeMeter;  // Cap the meter at its maximum value
                Debug.Log("Bullet Time Meter is Full!");
            }
            else
            {
                Debug.Log("Bullet Time Meter: " + bulletTimeMeter);
            }
        }
    }

    // Function to check if both the enemy and player are in the shadows
    bool CheckEnemyAndPlayerInShadow()
    {
        // Raycast to detect enemy in front of the player
        if (Physics.Raycast(takedownCheck.position, takedownCheck.forward, out RaycastHit hitInfo, 3f, enemyLayer))
        {
            // Check if enemy is in the shadow by using another raycast or sphere overlap check
            if (Physics.CheckSphere(hitInfo.transform.position, 1f, shadowLayer))
            {
                // Enemy is in shadow
                return true;
            }
        }

        // Check if player is in shadow using a sphere check at player position
        if (Physics.CheckSphere(takedownCheck.position, 1f, shadowLayer))
        {
            // Player is in shadow
            return true;
        }

        // If neither the enemy nor player is in the shadow, return false
        return false;
    }

    // Function to activate bullet time
    void ActivateBulletTime()
    {
        isBulletTimeActive = true;
        Time.timeScale = bulletTimeScale;  // Slow down time to 20%
        StartCoroutine(DeactivateBulletTimeAfterDelay());  // Start coroutine to deactivate after a delay
    }

    // Coroutine to deactivate bullet time after a set duration
    IEnumerator DeactivateBulletTimeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(bulletTimeDuration);  // Wait for real-time duration
        Time.timeScale = 1f;  // Reset time scale to normal speed
        isBulletTimeActive = false;  // Bullet time is no longer active
        bulletTimeMeter = 0f;  // Reset the bullet time meter after use
    }
}