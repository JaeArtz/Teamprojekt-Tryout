using UnityEngine;

public class BossDamageLogic : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Object needs Player Tag to trigger
        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
                Debug.Log("Boss hit Player during Stomp!");
            }
        }
    }
}