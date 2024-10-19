using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script that can be put on any object that needs to always rotate to face the player
public class billboardUI : MonoBehaviour
{
    
    private Camera playerCamera; //A new camera variable called playerCamera

    //A function that gets called at the start of a session
    private void Start()
    {
        playerCamera = Camera.main;// Connects the playerCamera to the main camera
    }

    //A function that is called every update
    private void Update()
    {
        LookAtPlayer();
    }

    //A function that is given the rotation and position corrdinates of the player camera, and rotates the given object based on those parameters
    private void LookAtPlayer()
    {
        transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.forward, playerCamera.transform.rotation * Vector3.up);
    }
}
