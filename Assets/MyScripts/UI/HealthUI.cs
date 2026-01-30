using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    public void UpdateHealthBar(float normalizedValue)
    {
        if (healthBar != null)
            healthBar.value = normalizedValue;
    }
}