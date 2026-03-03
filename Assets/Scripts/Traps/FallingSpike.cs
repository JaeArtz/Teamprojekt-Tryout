using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private BoxCollider2D physicsCollider; // Für den Boden
    [SerializeField] private BoxCollider2D triggerCollider; // Für den Schaden am Spieler
    public float speed;
    public float distance;
    bool isFalling = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.queriesStartInColliders = false;
        if(isFalling == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance);
            Debug.DrawRay(transform.position, Vector2.down * distance, Color.red);

            if(hit.transform != null)
            {
                if(hit.transform.CompareTag("Player"))
                {
                    rb.gravityScale = speed;
                    isFalling = true;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            rb.linearVelocity = Vector2.zero; 
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;

            Destroy(gameObject, 2f);
        }
    }

    //Einfach nur um ein Test push auf meinem Branch zu machen
}
