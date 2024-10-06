using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;

public class playerTakedown : MonoBehaviour
{
    [Header("Refernces")]

    [SerializeField] private KeyCode takedownKey;
    [SerializeField] LayerMask enemy;
    [SerializeField] private Transform takedownCheck;
    [SerializeField] private Text takedownText;

    private Color purple = Color.magenta;

    public bool canTakedown;
    public bool canSee;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        canTakedown = Physics.CheckSphere(takedownCheck.position, 3f, enemy);

        Debug.DrawRay(takedownCheck.position, takedownCheck.forward);

        takedownText.text = "Cannot Takedown";

        if (canTakedown && Physics.Raycast(takedownCheck.position, takedownCheck.forward, out RaycastHit hitInfo, 100, enemy))
        {
            takedownText.text = "Can Takedown";
            hitInfo.transform.GetComponentInChildren<MeshRenderer>().material.color = purple;

            if (Input.GetKeyDown(takedownKey))
            {
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>(); //Gets the components from the "IDamageable" interface, which for now is just the "damage" function
                
                damageable?.Damage(10000); //Calls the "damage" function with the gun's "damage" data as its only parameter
                Debug.Log(hitInfo.transform.name);
            }
                
            
            
        }
    }







    
}
