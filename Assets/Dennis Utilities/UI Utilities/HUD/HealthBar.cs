using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public PlayerHealth player;

    private void Start()
    {
        SetMaxHealth();
    }
    public void SetMaxHealth()
    {
        slider.maxValue = player.maxHealth;
        slider.value = player.currentHealth;

        
        fill.color = gradient.Evaluate(1.0f);
    }

    public void OnEnable()
    {
        PlayerHealth.OnPlayerDamaged += SetHealth;
    }

    public void OnDisable()
    {
        PlayerHealth.OnPlayerDamaged -= SetHealth;
    }
    public void SetHealth()
    {
        slider.value = player.currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
