using UnityEngine;
using UnityEngine.SceneManagement; // Import Scene Management for loading scenes

public class SceneSwapper : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Name of the scene to load

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Swapping to scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad); // Load the specified scene
        }
    }
}
