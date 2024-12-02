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
        SpawnPlayerAtPoint();
    }

    private void SpawnPlayerAtPoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player GameObject not found! Ensure the player exists in the scene and is tagged 'Player'.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint not assigned! Ensure a valid spawn point is set in the Inspector.");
            return;
        }

        // Reset the player's position and rotation
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

        // Handle Rigidbody or CharacterController reset (if applicable)
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Stop any existing movement
            rb.angularVelocity = Vector3.zero; // Stop any rotation
            Debug.Log("Player Rigidbody reset.");
        }

        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false; // Disable to reset position properly
            characterController.transform.position = spawnPoint.position;
            characterController.enabled = true; // Re-enable
            Debug.Log("Player CharacterController reset.");
        }

        Debug.Log($"Player successfully spawned at {spawnPoint.position}");
    }
}
