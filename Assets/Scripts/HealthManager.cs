using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    private static int playerHP;
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

            if(Input.GetKeyDown(respawnKey))
            {
                isGameOver = false;
                Instance.BecomeTemporarilyInvincible();
                Time.timeScale = 1;
                playerHP = maxHP;
                gameOverPanel.SetActive(false);
            }
        }
    }

    public static void Damage(int damageAmount)
    {
        
        if(isInvincible)
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
        if(playerHP < Instance.maxHP)
        {
            playerHP += regenAmount;

            if(playerHP > Instance.maxHP)
            {
                playerHP = Instance.maxHP;
            }
        }

        else
        {
            return;
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
        if (healthBarSlider.value < 21) {
            lowHealthOverlay.SetActive(true);
        }
        else {
            lowHealthOverlay.SetActive(false);
        }
    }

    private float CalculateHealthPercentage()
    {
        return ((float)playerHP / (float)maxHP) * 100;
    }
}
