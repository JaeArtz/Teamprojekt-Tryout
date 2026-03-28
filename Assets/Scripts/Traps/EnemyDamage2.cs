using UnityEngine;

public class EnemyDamage2 : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] protected int damage = 1;
    [SerializeField] private float hitCooldown = 1.0f;

    private float lastHitTime = -99f;
    private bool isInPlayer = false;
    private PlayerHealth playerHealth;

    private void FixedUpdate()
    {
       
        if (isInPlayer && (Time.time - lastHitTime >= hitCooldown))
        {
            if (playerHealth != null)
            {
                ApplyDamage();
            }
        }
    }

    private void ApplyDamage()
    {
        playerHealth.TakeDamage(damage);
        lastHitTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                isInPlayer = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayer = false;
            playerHealth = null;
        }
    }
}