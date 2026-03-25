using UnityEngine;

public class BossWave : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 1; // Halbes Herz
    private float direction;
    private float lifetime = 4f;

    public void Setup(float dir)
    {
        direction = dir;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Wir holen uns den Controller, um den Roll-Status zu prüfen
            PlayerController pc = collision.GetComponentInParent<PlayerController>();

            // HINWEIS: Du musst in deinem PlayerController eine 
            // Variable oder Eigenschaft "IsRolling" haben!
            if (pc != null && !pc.IsInputLocked())
            {
                // Hier die Abfrage: Wenn er NICHT rollt, kriegt er Schaden
                // Falls du das Flag noch nicht hast, musst du es im PlayerController ergänzen
                // if (!pc.isRolling) { ... }

                PlayerHealth health = collision.GetComponentInParent<PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    // Die Welle zerstört sich meist nicht beim Treffer, 
                    // damit sie weiterrollt, oder?
                }
            }
        }
    }
}