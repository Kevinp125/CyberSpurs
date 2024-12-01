using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionManager : MonoBehaviour
{
    public Transform spawnPoint; // Assign this to your spawn point in the Inspector

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure the player is placed at the correct spawn point
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation; // Match rotation if needed
        }
    }
}
