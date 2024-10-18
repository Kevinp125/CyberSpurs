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
    public Text gameOverText;
    public KeyCode respawnKey;

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
            gameOverText.text = "GAMEOVER";
            Time.timeScale = 0;

            if(Input.GetKeyDown(respawnKey))
            {
                gameOverText.text = "";
                Time.timeScale = 1;
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

    private IEnumerator BecomeTemporarilyInvincible()
    {
        
        isInvincible = true;

        yield return new WaitForSeconds(invincibilitySeconds);

        isInvincible = false;

    }

    private void SetHealhBarUI()
    {
        healthBarSlider.value = CalculateHealthPercentage();
    }

    private float CalculateHealthPercentage()
    {
        return ((float)playerHP / (float)maxHP) * 100;
    }
}
