using UnityEngine;

public class BossDamageLogic : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Wir reagieren nur, wenn das Objekt den Tag 'Player' hat
        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                Debug.Log("Boss hat Player während Stampfen getroffen!");
            }
        }
    }
}