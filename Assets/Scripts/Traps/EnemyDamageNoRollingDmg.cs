using UnityEngine;

public class EnemyDamageNoRollingDmg : MonoBehaviour

{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageInterval = 2.0f;
    [SerializeField] private float initialDelay = 0f;

    private float nextDamageTime;
    private bool isInside = false;

    private PlayerHealth playerHealth;
    private PlayerRoll playerRoll;

    private void Update()
    {
        if (isInside && playerHealth != null)
        {
            // Wir versuchen das Roll-Skript zu finden, falls wir es noch nicht haben
            if (playerRoll == null)
            {
                playerRoll = playerHealth.GetComponentInChildren<PlayerRoll>();
            }

            // Prüfung: Ist das Intervall abgelaufen?
            if (Time.time >= nextDamageTime)
            {
                // ZUSATZ: Nur Schaden machen, wenn der Spieler NICHT rollt
                // Wenn playerRoll nicht gefunden wird, machen wir sicherheitshalber Schaden
                bool isRolling = (playerRoll != null) && playerRoll.IsRolling;

                if (!isRolling)
                {
                    playerHealth.TakeDamage(damage);
                    nextDamageTime = Time.time + damageInterval;
                }
                else
                {
                    // Optional: Falls der Spieler im Gegner aufhört zu rollen, 
                    // soll er sofort Schaden bekommen. Deshalb setzen wir den Timer nicht hoch.
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Sucht PlayerHealth in der Parent-Hierarchie
            playerHealth = collision.GetComponentInParent<PlayerHealth>();

            // Sucht PlayerRoll (oft auf einem Child-Objekt wie "Roller" oder beim Health)
            playerRoll = collision.GetComponentInParent<PlayerRoll>();
            if (playerRoll == null)
                playerRoll = collision.GetComponentInChildren<PlayerRoll>();

            if (playerHealth != null)
            {
                isInside = true;
                nextDamageTime = Time.time + initialDelay;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInside = false;
            nextDamageTime = 0;
            playerHealth = null;
            playerRoll = null;
        }
    }
}