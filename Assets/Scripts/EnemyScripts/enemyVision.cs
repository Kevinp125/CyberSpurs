using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyVision : MonoBehaviour
{
    static public bool found = false;

    public float lookRadius;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, lookRadius);

        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.tag == "Player")
            {
                found = true;
            }

            else
            {
                found = false;
            }
        }

    }
}
