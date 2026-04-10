using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer parentSpriteRenderer;

    [Header("Audio Settings")]
    [Tooltip("Sound played as Warning before Spime falls down")]
    [SerializeField] private AudioSource audioSourceWarning;
    [Tooltip("Sound played on impact of FallDown and Splatter")]
    [SerializeField] private AudioSource audioSourceFallingCrush;

    [SerializeField] private LayerMask playerLayer;
    public float distance;
    public float speed;
    [Tooltip("Time until FallingSpike destroys itself after falling down")]
    public float delaySelfdestruct= 1f;
    bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        parentSpriteRenderer = GetComponent<SpriteRenderer>();
        // audioSourceWarning = GetComponent<AudioSource>();
        // audioSourceFallingCrush = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(isFalling == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance, playerLayer);
            Debug.DrawRay(transform.position, Vector2.down * distance, Color.red);

            if(hit.collider != null) // hit.collider reicht hier völlig aus
            {
                // 0. WARNING
                if (audioSourceWarning != null && audioSourceWarning.clip != null)
                {
                    audioSourceWarning.PlayOneShot(audioSourceWarning.clip);
                }

                rb.gravityScale = speed;
                isFalling = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground")) // Wenn der Spike den Boden berührt, wird er zerstört
        {
           
            // 1. FALL
            rb.linearVelocity = Vector2.zero; 
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;

            // 1.5 PARENTSPIKE TURNS INVISIBLE
            if (parentSpriteRenderer != null)
            { 
                parentSpriteRenderer.enabled = false;
            }

            // 2. SOUND
            if (audioSourceFallingCrush != null && audioSourceFallingCrush.clip != null)
            {
                audioSourceFallingCrush.PlayOneShot(audioSourceFallingCrush.clip);
            }

            // 3. ANIMATION
            if (animator != null) 
            {
                animator.SetTrigger("HitGround");
            }

            // 4. SELFDESTRUCT
            Destroy(gameObject, delaySelfdestruct);
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
