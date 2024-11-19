using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Destroy after a set time
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // Move forward
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit Player!");
            Destroy(gameObject); // Destroy projectile on impact
        }
    }
}
