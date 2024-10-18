using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{
    bool lookat = false;

    public GameObject player;
    Rigidbody RB;

    public float speed;
    public float multiplier;
    public float speedLimit;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyVision.found)
        {
            lookat = true;
        }

        if(lookat)
        {
            transform.LookAt(player.transform);

            Vector3 vel = RB.velocity;

            if(enemyVision.found && vel.x > -2 && vel.x < speedLimit && vel.z > -2 && vel.z < speedLimit)
            {
                RB.AddForce(speed * multiplier * Time.deltaTime * transform.forward);
            }
        }
    }
}
