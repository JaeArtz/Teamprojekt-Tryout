using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected int damage = 1;
    [SerializeField] private float damageInterval = 2.0f; // Alle 2 Sekunden Schaden
    [SerializeField] private float initialDelay = 0f; // Verzögerung vor dem ersten Schaden
    private float nextDamageTime;

    private bool isInside = false;
    protected PlayerHealth playerHealth;


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

    // Wir nutzen Enter für den ersten Kontakt
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealth = collision.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null && nextDamageTime <= (Time.time + initialDelay))
            {
                isInside = true;
                // den ersten Schadenszeitpunkt in die Zukunft
                nextDamageTime = Time.time + initialDelay;
            }
        }
    }

  
    // Timer zurücksetzen, wenn der Spieler den Gegner verlässt -> wenn man nochmal reingeht erneuter Schaden
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInside = false;
            nextDamageTime = 0;
        }
    }
}

