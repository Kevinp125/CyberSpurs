using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script that houses the shooting and reload keys
public class playerShoot : MonoBehaviour
{
    public static Action shootInput; //The input that calls the "shoot" function
    public static Action reloadInput; //The input that calls the "reload" function

    [SerializeField] private KeyCode reloadKey; //The key that the user presses to reload the gun

    //A function that gets called every "update" or frame
    private void Update()
    {
        //An if statement that is invokes the "shoot" function whenever the user presses left click
        if(Input.GetMouseButton(0))
        {
            shootInput?.Invoke();
        }

        //An if statement that invokes the "reload" function whenever the user specified reload key is pressed
        if (Input.GetKeyDown(reloadKey))
        {
            reloadInput?.Invoke();
        }
    }
}
