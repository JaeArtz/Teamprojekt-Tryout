using UnityEngine;
using System.Collections.Generic; // added for lists of Audio, instead one single clip

public class FallingSpike : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer parentSpriteRenderer;

    [Header("Audio Settings")]
    [Tooltip("Sound played as Warning before Spime falls down")]
    [SerializeField] private List<AudioClip> warningSounds;
    [Tooltip("Sound played on impact of FallDown and Splatter")]
    [SerializeField] private List<AudioClip> crushSounds;
    [SerializeField] private AudioSource audioSource; 
    [Range(0, 1)] public float volume = 1f; 

    [SerializeField] private LayerMask playerLayer;
    public float distance;
    public float speed;
    [Tooltip("Time until FallingSpike destroys itself after falling down")]
    public float delaySelfdestruct = 1f;
    bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        parentSpriteRenderer = GetComponent<SpriteRenderer>();
        // audioSourceWarning = GetComponent<AudioSource>();
        // audioSourceFallingCrush = GetComponent<AudioSource>();

        // We need an AudioSource, must check for it
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isFalling == false)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance, playerLayer);
            Debug.DrawRay(transform.position, Vector2.down * distance, Color.red);

            if (hit.collider != null) // hit.collider reicht hier völlig aus
            {
                // 0. WARNING
                PlayRandomSFX(warningSounds); // Geändert auf Zufallsfunktion

                rb.gravityScale = speed;
                isFalling = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Wenn der Spike den Boden berührt, wird er zerstört
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
            PlayRandomSFX(crushSounds); // Geändert auf Zufallsfunktion

            // 3. ANIMATION
            if (animator != null)
            {
                animator.SetTrigger("HitGround");
            }

            // 4. SELFDESTRUCT
            Destroy(gameObject, delaySelfdestruct);
        }
    }

    // Neu hinzugefügte Hilfsfunktion zur Zufallsauswahl
    private void PlayRandomSFX(List<AudioClip> sounds)
    {
        if (sounds != null && sounds.Count > 0 && audioSource != null)
        {
            AudioClip clip = sounds[Random.Range(0, sounds.Count)];
            audioSource.PlayOneShot(clip, volume);
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