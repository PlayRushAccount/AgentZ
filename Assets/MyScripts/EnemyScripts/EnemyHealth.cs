using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    private float currentHealth;

    public GameObject damagePopupPrefab;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
    Debug.Log($"{gameObject.name} took {amount} damage. Remaining: {currentHealth}");

    ShowDamagePopup(amount);

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

    private void ShowDamagePopup(float amount)
{
    if (damagePopupPrefab != null)
    {
        GameObject popup = Instantiate(damagePopupPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        DamagePopUp damagePopup = popup.GetComponent<DamagePopUp>();
        if (damagePopup != null)
        {
            damagePopup.Setup(amount);
        }
    }
}

}
