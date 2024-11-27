using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerTakedown : MonoBehaviour
{
    [Header("Refernces")]

    [SerializeField] private KeyCode takedownKey;
    [SerializeField] LayerMask enemyShadow;
    [SerializeField] LayerMask shadow;
    [SerializeField] private Transform takedownCheck;


    public bool canTakedown;

    public int healthRegen;

    private Transform lastEnemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Physics.Raycast(takedownCheck.position, takedownCheck.forward, out RaycastHit hitInfo, 3f, enemyShadow) && Physics.CheckSphere(takedownCheck.position, 1f, shadow))
        {
            TextMeshProUGUI enemyText = hitInfo.transform.GetComponentInChildren<TextMeshProUGUI>();

            if (enemyText != null)
                enemyText.text = "TakeDown!";

            lastEnemy = hitInfo.transform;

            if (Input.GetKeyDown(takedownKey))
            {
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>(); //Gets the components from the "IDamageable" interface, which for now is just the "damage" function
                
                damageable?.Damage(10000); //Calls the "damage" function with the gun's "damage" data as its only parameter
                Debug.Log(hitInfo.transform.name);

                HealthManager.Regen(healthRegen);
            }
                
            
            
        }

        else
        {
            // Reset the text of the last enemy if the player is out of range
            if (lastEnemy != null)
            {
                TextMeshProUGUI lastEnemyText = lastEnemy.GetComponentInChildren<TextMeshProUGUI>();
                if (lastEnemyText != null)
                {
                    lastEnemyText.text = "";
                }

                // Clear the reference to avoid resetting repeatedly
                lastEnemy = null;
            }
        }



    }







    
}
