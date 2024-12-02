using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    public static int playerHP;
    public int maxHP;
    public int invincibilitySeconds;

    public static bool isGameOver;
    public static bool isInvincible;

    public Slider healthBarSlider;
    public KeyCode respawnKey;

    public GameObject lowHealthOverlay;
    public AudioSource hurtAudio;
    public GameObject gameOverPanel;

    private void Awake()
    {
        // Ensure there's only one instance of HealthManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerHP = maxHP;
        isGameOver = false;
        isInvincible = false;
        SetHealhBarUI();
    }

    // Update is called once per frame
    void Update()
    {
        SetHealhBarUI();

        if (isGameOver)
        {
            Time.timeScale = 0;
            gameOverPanel.SetActive(true);

            if (Input.GetKeyDown(respawnKey))
            {
                RespawnPlayer();
            }
        }
    }

    public static void Damage(int damageAmount)
    {
        if (isInvincible)
        {
            return;
        }
        else
        {
            playerHP -= damageAmount;

            if (playerHP <= 0)
            {
                isGameOver = true;
            }

            Instance.StartCoroutine(Instance.BecomeTemporarilyInvincible());
        }
    }

    public static void Regen(int regenAmount)
    {
        if (playerHP < Instance.maxHP)
        {
            playerHP += regenAmount;

            if (playerHP > Instance.maxHP)
            {
                playerHP = Instance.maxHP;
            }
        }
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        hurtAudio.Play();
        isInvincible = true;

        yield return new WaitForSeconds(invincibilitySeconds);

        isInvincible = false;
    }

    private void SetHealhBarUI()
    {
        healthBarSlider.value = CalculateHealthPercentage();
        if (healthBarSlider.value < 21)
        {
            lowHealthOverlay.SetActive(true);
        }
        else
        {
            lowHealthOverlay.SetActive(false);
        }
    }

    private float CalculateHealthPercentage()
    {
        return ((float)playerHP / (float)maxHP) * 100;
    }

private void RespawnPlayer()
{
    isGameOver = false;
    Time.timeScale = 1;
    playerHP = maxHP;

    // Reset UI
    gameOverPanel.SetActive(false);
    SetHealhBarUI();

    // Find the spawn point dynamically in the current scene
    GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
    Debug.Log($"Spawn Point Found: {spawnPoint != null}");

    if (spawnPoint != null)
    {
        GameObject player = GameObject.FindWithTag("Player");
        Debug.Log($"Player Found: {player != null}");
        
        if (player != null)
        {
            // Reset Rigidbody
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.position = spawnPoint.transform.position; // Update Rigidbody position
                rb.rotation = Quaternion.identity; // Reset rotation if needed
                rb.isKinematic = false;
            }


            // Reset CharacterController or Collider
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = spawnPoint.transform.position;
                controller.enabled = true;
            }

                CapsuleCollider collider = player.GetComponent<CapsuleCollider>();
                if (collider != null)
                {
                    collider.enabled = false;
                    player.transform.position = spawnPoint.transform.position;
                    collider.enabled = true;
                }

                // Force physics update
                player.transform.position = spawnPoint.transform.position;
                Physics.SyncTransforms();

                Debug.Log($"Player moved to spawn point at: {spawnPoint.transform.position}");
            }
        else
            {
                Debug.LogError("Player object not found!");
            }
        }
        else
        {
            Debug.LogError("Spawn point not found in the current scene!");
        }

        // Make the player temporarily invincible after respawn
        StartCoroutine(BecomeTemporarilyInvincible());
    }


}
