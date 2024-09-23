using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script that creates a new types of asset and fills out all the info for the gun
[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")] //Creates the "gun" gameObject as an option when creating a new asset
public class gunData : ScriptableObject
{
    //The area where the name of the gun will be filled out
    [Header("Info")]
    public new string name;

    //The area where the damage of each shot and how far each bullet goes will be filled out
    [Header("Shooting")]
    public float damage;
    public float maxDistance;

    //The area where the size of the magazine, fire rate, and time for each reload will be filled out
    [Header("Reloading")]
    public int magSize;
    public float fireRate;
    public float reloadTime;

    //A boolean variable that indicates when the gun is reloading that is hidden from view in the inspector
    [HideInInspector]
    public bool reloading;
}
