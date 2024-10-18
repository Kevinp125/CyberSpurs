using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    private static int playerHP;
    public int maxHP;
    public int invincibilitySeconds; 

    public static bool isGameOver;
    public bool isInvincible;

    public Slider healthBarSlider;

    public Text gameOverText;

    public KeyCode respawnKey;

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
        playerHP -= damageAmount;

        if(playerHP <= 0)
        {
            isGameOver = true;
        }


        StartCoroutine(BecomeTemporarilyInvincible());


    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        
        isInvincible = true;

        yield return new WaitForSeconds(invincibilitySeconds);

        isInvincible = false;
\
    }

    private void SetHealhBarUI()
    {
        healthBarSlider.value = CalculateHealthPercentage();
    }

    private float CalculateHealthPercentage()
    {
        return (playerHP / maxHP) * 100;
    }
}
