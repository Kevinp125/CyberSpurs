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
    public bool isPlayerInShadows { get; private set; }  // Public read, private write. Setting this bool to use in other scripts
    [SerializeField] private KeyCode activationKey = KeyCode.B;
    [SerializeField] private Transform takedownCheck;  // Position to raycast from (e.g., player)
    [SerializeField] private LayerMask enemyLayer;  // LayerMask for enemies
    [SerializeField] private LayerMask shadowLayer;  // LayerMask for shadows
    [SerializeField] private float bulletTimeScale = 0.2f;  // LayerMask for shadows
    [SerializeField] private float bulletTimeMeterIncrease = 20f;   

    //below variables all have to do with bullettime FOV changes

    [SerializeField] private Camera playerCamera;  // Reference to the player's camera
    [SerializeField] private float normalFOV = 60f;  // Normal field of view
    [SerializeField] private float bulletTimeFOV = 80f;  // Wider field of view for bullet time
    [SerializeField] private float fovTransitionSpeed = 2f;  // Speed at which FOV changes


 
    // UI elements (optional)
    public Slider bulletTimeBar;  // Progress bar for the bullet time meter

    public Image kadePortrait;  // HUD image object showing Kade's portrait and visibility status
    public Sprite kadeVisible;  // Kade's visible sprite portrait asset
    public Sprite kadeHidden;   // Kade's hidden sprite portrait asset

    void Update()
    {
        // Check if the player presses "B" and the bullet time meter is full
        if (Input.GetKeyDown(activationKey) && bulletTimeMeter >= maxBulletTimeMeter && !isBulletTimeActive)
        {
            ActivateBulletTime();  // Activate bullet time when the meter is full
        }

        if (isBulletTimeActive)
        {
            // Smooth transition to bullet time FOV
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, bulletTimeFOV, fovTransitionSpeed * Time.deltaTime);
        }
        else
        {
            // Smooth transition back to normal FOV
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, fovTransitionSpeed * Time.deltaTime);
        }
        
        // Update the UI for the bullet time meter (optional)
        if (bulletTimeBar != null)
        {
            bulletTimeBar.value = bulletTimeMeter; //update progress bar
            bulletTimeBar.maxValue = maxBulletTimeMeter;
        }

        CheckIfKadeInShadow();



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

    public void IncreaseBulletTime(float percent) //this function is called when fighting the boss since we inccrease bulllet time meter depending on if we damage boss outside of shadows not if we kill it.
    {
        if (CheckEnemyAndPlayerInShadow())
        {
            // Do nothing or handle shadow case (optional)
            Debug.Log("Enemy is in the shadow, no bullet time increase.");
        }
        
        else{

            // Increase the bullet time meter based on the given percentage
            bulletTimeMeter += maxBulletTimeMeter * (percent / 100f);

            // Clamp the value to ensure it doesn't exceed the maximum
            bulletTimeMeter = Mathf.Clamp(bulletTimeMeter, 0f, maxBulletTimeMeter);

            // Update the UI
            if (bulletTimeBar != null)
            {
                bulletTimeBar.value = bulletTimeMeter;
            }

            Debug.Log("Bullet Time Meter: " + bulletTimeMeter);
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

    void CheckIfKadeInShadow(){
        if (Physics.CheckSphere(takedownCheck.position, 1f, shadowLayer))
        {
            kadePortrait.sprite = kadeHidden;
            isPlayerInShadows = true;
        }

        else
        {
            kadePortrait.sprite = kadeVisible;
            isPlayerInShadows = false;
        }

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