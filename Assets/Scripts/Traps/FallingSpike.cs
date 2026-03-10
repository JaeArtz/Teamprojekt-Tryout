using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private PolygonCollider2D physicsCollider; // Für den Boden
    [SerializeField] private PolygonCollider2D triggerCollider; // Für den Schaden am Spieler
    public float speed;
    public float distance;
    bool isFalling = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Physics2D.queriesStartInColliders = false; //Damit der Raycast nicht mit dem eigenen Collider kollidiert
        if(isFalling == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance);
            //Debug.DrawRay(transform.position, Vector2.down * distance, Color.red);

            if(hit.transform != null)
            {
                if(hit.transform.CompareTag("Player")) //Wenn Raycsast den Spieler trifft, fällt der Spike
                {
                    rb.gravityScale = speed;
                    isFalling = true;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground")) // Wenn der Spike den Boden berührt, wird er zerstört
        {
            rb.linearVelocity = Vector2.zero; 
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;

            Destroy(gameObject, 1f);
        }
    }

    // Zeigt die Reichweite im Editor an
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * distance;

        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireCube(endPos, new Vector3(0.5f, 0.1f, 0));
    }
}
