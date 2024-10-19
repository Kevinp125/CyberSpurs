using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;

public class playerTakedown : MonoBehaviour
{
    [Header("Refernces")]

    [SerializeField] private KeyCode takedownKey;
    [SerializeField] LayerMask enemyShadow;
    [SerializeField] LayerMask shadow;
    [SerializeField] private Transform takedownCheck;
    [SerializeField] private Text takedownText;

    

    public bool canTakedown;

    public int healthRegen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        takedownText.text = "Cannot Takedown";


        if (Physics.Raycast(takedownCheck.position, takedownCheck.forward, out RaycastHit hitInfo, 3f, enemyShadow) && Physics.CheckSphere(takedownCheck.position, 1f, shadow))
        {
            takedownText.text = "Can Takedown";

            if (Input.GetKeyDown(takedownKey))
            {
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>(); //Gets the components from the "IDamageable" interface, which for now is just the "damage" function
                
                damageable?.Damage(10000); //Calls the "damage" function with the gun's "damage" data as its only parameter
                Debug.Log(hitInfo.transform.name);

                HealthManager.Regen(healthRegen);
            }
                
            
            
        }


    }







    
}
