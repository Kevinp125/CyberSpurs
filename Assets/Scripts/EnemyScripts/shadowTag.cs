using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowTag : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask shadow;
    public Material outlineMaterial; 
    private Material originalMaterial;
    private Renderer renderer;

    public bool insideShadow;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material; // Save the original material
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.CheckSphere(transform.position, 1f, shadow))
        {
            gameObject.layer = 8;
            insideShadow = true;
        }

        else
        {
            gameObject.layer = 6;
            insideShadow = false;
        }

        if(insideShadow == true)
        {

        }
           
    }

}
