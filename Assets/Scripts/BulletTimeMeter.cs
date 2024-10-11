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
        if (!isInShadows)  // Only increase the meter if the player is outside the shadows
        {
            bulletTimeMeter += 20f;  // Increase the meter by 20 per kill (adjust as needed)
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

    // Call this function to set the player's shadow status (from your shadow detection logic)
    public void SetInShadows(bool inShadows)
    {
        isInShadows = inShadows;  // Update the player's shadow state
    }

    // Function to activate bullet time
    void ActivateBulletTime()
    {
        isBulletTimeActive = true;
        Time.timeScale = 0.2f;  // Slow down time to 20%
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