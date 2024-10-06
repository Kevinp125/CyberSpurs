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

    public bool canTakedown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        canTakedown = Physics.CheckSphere(takedownCheck.position, 3f, enemy);

        takedownText.text = "Cannot Takedown";

        if (canTakedown)
        {
            Collider[] takedownColliders = Physics.OverlapSphere(takedownCheck.position, 3f, enemy);

            Debug.Log("Array Length" + takedownColliders.Length);

            takedownText.text = "Can Takedown";

            if (Input.GetKeyDown(takedownKey))
            {
                IDamageable damageable = takedownColliders[takedownColliders.Length - 1].GetComponent<IDamageable>();
                damageable?.Damage(10000);
            }
        }
    }







    
}
