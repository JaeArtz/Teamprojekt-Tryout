using UnityEngine;

public class FootBottomDamage : MonoBehaviour
{
    [SerializeField] private float damage = 40f;
    [SerializeField] private float damageInterval = 1f;
    private float lastDamageTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamage(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Falls der Player unter der Sohle gefangen ist
        if (other.CompareTag("Player"))
        {
            ApplyDamage(other.gameObject);
        }
    }

    private void ApplyDamage(GameObject player)
    {
        if (Time.time >= lastDamageTime + damageInterval)
        {
            // Hier wird dein bestehendes EnemyDamage System oder Health System aufgerufen
            var enemyDamage = GetComponent<EnemyDamage>();
            if (enemyDamage != null)
            {
                // Nutze dein vorhandenes Skript, falls es noch da ist
                Debug.Log("Sohle macht Schaden über EnemyDamage.");
            }
            else
            {
                Debug.Log("Sohle macht direkt Schaden.");
            }

            lastDamageTime = Time.time;
        }
    }
}