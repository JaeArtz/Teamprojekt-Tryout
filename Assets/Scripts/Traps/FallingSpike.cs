using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private LayerMask playerLayer;
    public float distance;
    public float speed;
    bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(isFalling == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance, playerLayer);
            Debug.DrawRay(transform.position, Vector2.down * distance, Color.red);

            if(hit.collider != null) // hit.collider reicht hier völlig aus
            {
                rb.gravityScale = speed;
                isFalling = true;
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
