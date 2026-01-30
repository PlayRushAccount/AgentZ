using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
     [Header("Health Settings")]
    public float maxHealth = 100f;

    private float currentHealth;

    

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
    Debug.Log($"{gameObject.name} took {amount} damage. Remaining: {currentHealth}");

   

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Play death animation, particle, or sound here
        Destroy(gameObject, 1f);
    }

    
}
