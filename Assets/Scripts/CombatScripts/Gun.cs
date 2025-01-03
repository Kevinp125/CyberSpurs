using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour //A script that houses all the basic functions of a gun, like shooting, reloading, and how much ammo and mag size a gun has at that moment
{
    //Fields that house references to the gunData script and to the "muzzle" of the gun
    [Header("References")]
    [SerializeField] private gunData gunData; //The gunData script is a script that houses all the data that the "Gun" scirpt then uses to make functions for the fun. Things such as how much damage each shot takes, how many shots are in one mag, etc.
    [SerializeField] private Transform muzzle; //The "muzzle" is just an empty GameObject that acts as a point for the gun to know where the bullet comes out
    [SerializeField] private Transform hitScan; //variable is to assign the correct camera so that the hitscan is right when the player sho
    [SerializeField] private GameObject bulletPrefab; // Reference to the bullet prefab (your bullet asset)
    [SerializeField] private float bulletSpeed = 20f; // Speed of the bullet
    [SerializeField] private LayerMask layerIgnore; // Speed of the bullet

    //Variables that house the time since the last shot taken, and the amount of ammo currently in the gun
    float timeSinceLastShot;
    public float localAmmo;

    public TextMeshProUGUI ammoText;

    public AudioSource shootAudio;      // Shoot sound effect
    public AudioSource reloadAudio;     // Reload sound effect

    //A method that only activates at the start of a session
    private void Start()
    {
        localAmmo = gunData.magSize;//Sets the currant amount of ammo to the amount of ammo that fits in a magazine
        playerShoot.shootInput += Shoot;//Activates the shoot function
        playerShoot.reloadInput += StartReload;//Activates the reload function
    }

    //A function that starts the reload coroutine
    public void StartReload()
    {
        //An if statement that allows the gun to reload if the "reloading" boolean variable is set to false
        if (!gunData.reloading)
        {
            StartCoroutine(Reload());
        }
    }

    // A function that reloads the gun with new ammo
    private IEnumerator Reload()
    {
        reloadAudio.Play();     // Play the reload sound effect
        gunData.reloading = true; //Sets the "reloading" boolean variable to true for the duration of the reloading phase
        ammoText.text = "Reloading..."; //Sets the ammo text to show "reloading"

        yield return new WaitForSeconds(gunData.reloadTime); //Does not allow any gun actions to occur for the duration of the user specified reload time

        localAmmo = gunData.magSize; //Sets the current ammo of the gun back to maximum
        gunData.reloading = false; //Sets the "reloading" boolean variable back to false to allow the player to reload again if they want to

        
        textUpdate(); //Calls upon the "textUpdate" function after reloading is done
    }

    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f); // A boolean function that only allows the gun to shoot if the gun is not "reloading" and if the "timesinceLastShot" variable is higher than the user specified firerate

    //A function that shoots a round and damages whatever is in front of the muzzle
    public void Shoot()
    {
        //An if statement that allows the gun to shoot if the current amount of ammo in the gun is greater than 0
        if (localAmmo > 0 && CanShoot())
        {
           Ray ray = new Ray(hitScan.position, hitScan.forward);
           Vector3 targetPoint;

            // Perform raycast to detect if there's an object in front of the crosshair
            if (Physics.Raycast(ray, out RaycastHit hitInfo, gunData.maxDistance, layerIgnore))
            {
                targetPoint = hitInfo.point;  // If something is hit, set the target point to the hit position
            }
            else
            {
                targetPoint = ray.GetPoint(gunData.maxDistance);  // If nothing is hit, use a far point
            }

            // Calculate the direction from the bullet exit to the target point
            Vector3 directionToTarget = (targetPoint - muzzle.position).normalized;

            // Instantiate the bullet at the bulletExit point and make it face the target direction
            GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(directionToTarget));
            Rigidbody rb = bullet.GetComponent<Rigidbody>(); //gets the rigidbody component of the bullet

            if (rb != null) //if this doesnt return null it means bullet has rigid body so give it some velocity so that it launches forward
            { 
                rb.velocity = directionToTarget * bulletSpeed; //multiplying it by the correct vector we got from the ray cast fires the bullet towards the crosshair
            }
        
            //An if statement that allows the gun to "damage" a target if the target is within the calculated max distance
            if (Physics.Raycast(ray, out hitInfo, gunData.maxDistance, layerIgnore))
            {
                IDamageable damageable = hitInfo.transform.GetComponent<IDamageable>(); //Gets the components from the "IDamageable" interface, which for now is just the "damage" function
                damageable?.Damage(gunData.damage); //Calls the "damage" function with the gun's "damage" data as its only parameter

                            // Check if the hit object is an EnemyPatrol (to trigger aggro)
                EnemyPatrol enemy = hitInfo.transform.GetComponent<EnemyPatrol>();
                if (enemy != null)
                {
                    enemy.TakeDamage(); // Trigger the aggro behavior
                }

                EnemyPatrolChaseShoot chaseShootEnemy = hitInfo.transform.GetComponent<EnemyPatrolChaseShoot>();
                if (chaseShootEnemy != null)
                {   
                    chaseShootEnemy.TakeDamage(); // Trigger the aggro behavior for EnemyPatrolChaseShoot
                }
                
                Debug.Log(hitInfo.transform.name);
            }

            localAmmo--; // Removes one round from the current ammo
            timeSinceLastShot = 0; //Sets the "timeSinceLastShot" variable to 0, making it so the gun cannot shoot again until enough time has passed
            OnGunShot(); // A function that will activate once the gun is shot 

            shootAudio.Play();      // Play the gunshot sound effect
        }
    }

    //A function that gets called every "update" or frame
    private void Update()
    {
        //Adds time to the "timeSinceLastShot" variable
        timeSinceLastShot += Time.deltaTime;

        

        //Draws a vector from the gun's muzzle to indicate where it is pointing
        Debug.DrawRay(muzzle.position, muzzle.forward);
    }

    //A function that updates the ammo text whenever the gun is shooting or reloading
    private void textUpdate()
    {
        
        ammoText.text = localAmmo + " / " + gunData.magSize; //Converts the local ammo and the mag size of the gun into text
    }

    //A function that will get called everytime the gun is shot
    private void OnGunShot()
    {
        textUpdate();
    }
}
