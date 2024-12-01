using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats; // Reference to PlayerStats for stats
    public GameObject endStatsPanel; // The UI panel to display stats
    public Text starsEarnedText; // Text to display the number of stars earned
    public Text enemiesKilledText; // Text to display enemies killed
    public Text damageTakenText; // Text to display damage taken

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
        // Dynamically find the PlayerStats component on the player
        ReassignPlayerStats();

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
        // Reassign the PlayerStats reference whenever a new scene is loaded
        ReassignPlayerStats();
    }

    private void ReassignPlayerStats()
    {
        // Find the player object and get its PlayerStats component
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats component not found on Player GameObject!");
            }
        }
        else
        {
            Debug.LogError("Player GameObject not found in the scene!");
        }
    }

    // Method to calculate and display stats at the end of the level
    public void DisplayEndStats()
    {
        // Calculate stars
        CalculateStars();

        // Activate the stats panel
        // endStatsPanel.SetActive(true);

        // Update stats and stars text
        // enemiesKilledText.text = $"Enemies Killed: {playerStats.EnemiesKilled}";
        // damageTakenText.text = $"Damage Taken: {(playerStats.TookDamage ? "Yes" : "No")}";
        // starsEarnedText.text = $"Stars Earned: {starsEarned} / 3";

        Debug.Log($"Enemies Killed: {playerStats.EnemiesKilled}");
        Debug.Log($"Damage Taken: {(playerStats.TookDamage ? "Yes" : "No")}");
        Debug.Log($"Stars Earned: {starsEarned} / 3");
    }

    private void CalculateStars()
    {
        // Reset stars
        starsEarned = 0;

        // Award stars based on kills
        if (playerStats.EnemiesKilled >= 20) starsEarned = 1;
        if (playerStats.EnemiesKilled >= 30) starsEarned = 2;

        // Award additional star for no damage
        if (!playerStats.TookDamage) starsEarned++;
    }
}
