using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    //The header for the sway settings in Unity
    [Header("Sway Settings")]
    [SerializeField] private float smooth; //A setting for how smooth the weapon sway is (0 smooth means no sway)
    [SerializeField] private float swayMultiplier; //A setting for how much the weapon sways when the camera moves (The higher this value is, the more the weapon will sway)


    // Update is called once per frame
    void Update()
    {
        //Variables that grab the raw X and Y coordinates of the mouse every frame
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        // Quarternions(basically rotation value of an object in 3D) that are calculated by using the previous X and Y coordinates and a 3D axis
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        //This is the quaternion that determines how much the gun is supposed to rotate as the camera moves
        Quaternion targetRotation = rotationX * rotationY;

        //A function that rotates the gun based of the previous quaternion calculations
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}

