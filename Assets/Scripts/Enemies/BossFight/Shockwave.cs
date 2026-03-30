using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [Header("Einstellungen")]
    public float speed = 8f;        // Wie schnell rast die Welle?
    public float lifetime = 1.5f;   // Wann verschwindet sie? (Reichweite)
    public int damage = 1;          // 1 = Ein halbes Herz

    private int direction = 1;      // 1 = Rechts, -1 = Links
    private bool hasHitPlayer = false; // Sicherung gegen Mehrfachtreffer

    public void Setup(int dir)
    {
        direction = dir;

        // Spiegelt das Sprite (X-Achse)
        transform.localScale = new Vector3(dir, 1, 1);

        // Zerstört das Objekt automatisch nach Ablauf der Lebenszeit
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Konstante Bewegung in die zugewiesene Richtung
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nur Schaden machen, wenn es der Player ist UND wir ihn noch nicht getroffen haben
        if (other.CompareTag("Player") && !hasHitPlayer)
        {
            var health = other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                hasHitPlayer = true; // Sperre für DIESE Welle aktivieren
                Debug.Log("Schockwelle Treffer: Einmalig " + damage + " Schaden verursacht.");
            }
        }
    }
}