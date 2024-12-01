using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence instance; // Singleton instance to track the player

    void Awake()
    {
        // If there's already an instance, destroy the duplicate
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy the new player object
            return;
        }

        // Otherwise, make this the instance and preserve it
        instance = this;
        DontDestroyOnLoad(gameObject); // Preserve this player object across scenes
    }
}
