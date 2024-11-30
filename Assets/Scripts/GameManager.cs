using UnityEngine;
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

    // Method to calculate and display stats at the end of the level
    public void DisplayEndStats()
    {
        // Calculate stars
        CalculateStars();

        // Activate the stats panel
        endStatsPanel.SetActive(true);

        // Update stats and stars text
        enemiesKilledText.text = $"Enemies Killed: {playerStats.EnemiesKilled}";
        damageTakenText.text = $"Damage Taken: {(playerStats.TookDamage ? "Yes" : "No")}";
        starsEarnedText.text = $"Stars Earned: {starsEarned} / 3";

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
