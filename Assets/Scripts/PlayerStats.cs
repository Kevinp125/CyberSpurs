using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int EnemiesKilled { get; private set; }
    public bool TookDamage { get; private set; }

    public void AddKill()
    {
        EnemiesKilled++;
        Debug.Log($"Enemies Killed: {EnemiesKilled}");
    }

    public void TakeDamage()
    {
        if (!TookDamage) // Only log the first time damage is taken
        {
            TookDamage = true;
            Debug.Log($"Player took damage! TookDamage flag set to: {TookDamage}");
        }
    }
}
