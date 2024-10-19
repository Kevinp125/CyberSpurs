using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script that gives an object health, and implements the "IDamageable" interface to let it be damaged with a gun shot
public class Target : MonoBehaviour , IDamageable
{ 
    public float health = 100f;//A variable that determines the target's health

    //A function that "damages" the target
    public void Damage(float damage)
    {
        health -= damage; //Subtracts the amount of damage the user specifies from the target's health

        //An if statement that destroy's the target gameobject if the health gets to 0 or less
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
