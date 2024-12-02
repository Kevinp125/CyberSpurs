using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats; // Reference to PlayerStats for stats
    public GameObject endStatsPanel; // The UI panel to display stats
    public TMP_Text starsEarnedText; // Text to display the number of stars earned
    public TMP_Text enemiesKilledText; // Text to display enemies killed
    public TMP_Text damageTakenText; // Text to display damage taken

    public playerController playerController; // Reference to PlayerController
    public playerShoot playerShoot; // Reference to PlayerShoot
    private int starsEarned = 0;

    private static GameManager instance; // Singleton instance

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager
        }
    }

    private void Start()
    {
        // Dynamically find the PlayerStats, PlayerController, and PlayerShoot components on the player
        ReassignPlayerReferences();

        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the scene loaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign references whenever a new scene is loaded
        ReassignPlayerReferences();
    }

    private void ReassignPlayerReferences()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Reassign PlayerStats
            playerStats = player.GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats component not found on Player GameObject!");
            }

            // Reassign PlayerController
            playerController = player.GetComponent<playerController>();
            if (playerController == null)
            {
                Debug.LogError("playerController component not found on Player GameObject!");
            }

            // Reassign PlayerShoot
            playerShoot = player.GetComponent<playerShoot>();
            if (playerShoot == null)
            {
                Debug.LogError("playerShoot component not found on Player GameObject!");
            }
        }
        else
        {
            Debug.LogError("Player GameObject not found in the scene!");
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restart button clicked!"); // Debug log to confirm the button is clicked.
        // Reset player stats if the reference is valid
        if (playerStats != null)
        {
            playerStats.ResetStats();
        }

        // Reload Level 1
        SceneManager.LoadScene("Daniel_KeyScene");

        endStatsPanel.SetActive(false);

        // Reset cursor state
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        HealthManager.Regen(100);

        playerController.enabled = true;
        playerShoot.enabled = true;
        
    }

    public void DisplayEndStats()
    {
        Debug.Log("DisplayEndStats called.");

        // Calculate stars
        CalculateStars();

        // Activate the stats panel
        endStatsPanel.SetActive(true);

        // Enable the cursor for UI interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Disable player controls
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Disable player shooting
        if (playerShoot != null)
        {
            playerShoot.enabled = false;
        }

        // Update stats and stars text
        enemiesKilledText.text = $" {playerStats.EnemiesKilled}";
        damageTakenText.text = $" {(playerStats.TookDamage ? "Yes" : "No")}";
        starsEarnedText.text = $" {starsEarned} / 3";

        Debug.Log($"Enemies Killed: {playerStats.EnemiesKilled}");
        Debug.Log($"Damage Taken: {(playerStats.TookDamage ? "Yes" : "No")}");
        Debug.Log($"Stars Earned: {starsEarned} / 3");
    }

    private void CalculateStars()
    {
        // Reset stars
        starsEarned = 0;

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is null! Cannot display end stats.");
            return;
        }
        // Award stars based on kills
        if (playerStats.EnemiesKilled >= 20) starsEarned = 1;
        if (playerStats.EnemiesKilled >= 30) starsEarned = 2;

        // Award additional star for no damage
        if (!playerStats.TookDamage) starsEarned++;
    }
}
