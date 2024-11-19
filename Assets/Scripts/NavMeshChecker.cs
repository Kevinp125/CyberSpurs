using UnityEngine;
using UnityEngine.AI;

public class NavMeshChecker : MonoBehaviour
{
    public Transform player; // Assign your player Transform in the Inspector
    public float checkRadius = 0.5f; // Radius within which to check for the NavMesh

    void Update()
    {
        if (IsOnNavMesh(player.position))
        {
            Debug.Log("Player is on the NavMesh");
        }
        else
        {
            Debug.Log("Player is NOT on the NavMesh");
        }
    }

    private bool IsOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        // Check if the position is on the NavMesh within the checkRadius
        if (NavMesh.SamplePosition(position, out hit, checkRadius, NavMesh.AllAreas))
        {
            // Optional: You can also compare hit.position and the original position for precision
            return true;
        }
        return false;
    }
}
