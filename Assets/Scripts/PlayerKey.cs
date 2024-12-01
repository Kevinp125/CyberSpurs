using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerKey : MonoBehaviour
{
    public GameObject key; // Dynamically assigned
    public GameObject door; // Dynamically assigned
    public LayerMask layerIgnore;
    public bool haveKey;
    public GameObject keyActivatedEnemiesParent; // Parent GameObject for the enemies
    private List<GameObject> enemies = new List<GameObject>(); // List of enemies under the parent

    void Start()
    {
        haveKey = false;

        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign key, door, and enemies for the new scene
        key = GameObject.FindWithTag("Key");
        door = GameObject.FindWithTag("Door");

        if (key == null)
        {
            Debug.LogWarning("Key not found in the scene! Ensure a GameObject with the 'Key' tag exists.");
        }

        if (door == null)
        {
            Debug.LogWarning("Door not found in the scene! Ensure a GameObject with the 'Door' tag exists.");
        }

        // Find the parent GameObject for the key-activated enemies
        keyActivatedEnemiesParent = GameObject.Find("Key_Activated_Enemies");
        enemies.Clear(); // Clear the old list to avoid duplicates

        if (keyActivatedEnemiesParent != null)
        {
            foreach (Transform child in keyActivatedEnemiesParent.transform)
            {
                enemies.Add(child.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("Key_Activated_Enemies parent GameObject not found! Ensure it exists in the scene.");
        }
    }

    void Update()
    {
        // Check if the player is near the key
        if (Physics.CheckSphere(transform.position, 2f, layerIgnore))
        {
            if (key != null)
            {
                Destroy(key);
            }

            if (door != null)
            {
                Destroy(door);
            }

            haveKey = true;

            // Activate all enemies under Key_Activated_Enemies
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.SetActive(true); // Activate the enemy
                }
            }
        }
    }
}
