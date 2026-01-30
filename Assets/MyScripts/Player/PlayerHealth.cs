using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Optional")]
    [Tooltip("Assign a UI health bar or text updater here if you want visual feedback.")]
    public HealthUI healthUI;

    //private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        //animator = GetComponent<Animator>();

        if (healthUI != null)
            healthUI.UpdateHealthBar(currentHealth / maxHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Player took {damageAmount} damage. Current HP: {currentHealth}");

        if (healthUI != null)
            healthUI.UpdateHealthBar(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        //animator.SetTrigger("Die");
        // Additional death logic (disable controls, play sound, etc.) can be added here.
        //GetComponent<Shooting>()?.enabled = false;

    }
}
