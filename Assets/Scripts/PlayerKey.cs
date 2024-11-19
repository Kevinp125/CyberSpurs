using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKey : MonoBehaviour
{
    public GameObject key;
    public GameObject door;
    public LayerMask layerIgnore;
    public bool haveKey;
    // Start is called before the first frame update
    void Start()
    {
        haveKey = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.CheckSphere(transform.position, 2f, layerIgnore))
        {
            Destroy(key);
            Destroy(door);
            haveKey = true;
        }
    }
}
