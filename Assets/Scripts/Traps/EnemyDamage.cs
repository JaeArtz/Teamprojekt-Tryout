using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected int damage = 1;
    [SerializeField] private float damageInterval = 2.0f; // Alle 2 Sekunden Schaden
    [SerializeField] private bool canBeDodgedByRolling = true;
    private float nextDamageTime;

    private bool isInside = false;
    private PlayerHealth playerHealth;
    private PlayerRoll playerRoll;

    private void Update()
    {
        if (isInside && playerHealth != null)
        {
            // Nur Schaden zufügen, wenn das Intervall abgelaufen ist
            if (Time.time >= nextDamageTime)
            {
                playerHealth.TakeDamage(damage);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Nur Schaden zufügen, wenn das Intervall abgelaufen ist
            if (Time.time >= nextDamageTime)
            {
                playerHealth = collision.GetComponentInParent<PlayerHealth>();
                if (playerHealth != null)
                {
                    isInside = true;
                    playerHealth.TakeDamage(damage);
                    
                    // Timer für den nächsten Schaden setzen
                    nextDamageTime = Time.time + damageInterval;
                }
            }
        }
    }

    // Timer zurücksetzen, wenn der Spieler den Gegner verlässt -> wenn man nochmal reingeht erneuter Schaden
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInside = false;
            nextDamageTime = 0;
        }
    }
}

