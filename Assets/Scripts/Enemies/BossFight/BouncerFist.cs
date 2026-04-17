using UnityEngine;
using System.Collections;

public class BouncerFist : MonoBehaviour
{
    [Header("--- Targeting ---")]
    [Tooltip("Drag Player here, he is the Target/Trigger")]
    public Transform playerTransform;
    [Tooltip("This is how far the BouncerFist can reach, it stops here")]
    public Transform arenaLeftBoundary;

    [Header("--- Timing ---")]
    public float strikeDuration = 0.12f;
    public float stayDuration = 0.5f;
    public float retractDuration = 1.0f;

    [Header("--- Combat ---")]
    public int damage = 2;
    public float knockbackForce = 25f;
    public float upwardForce = 7f;
    public AudioSource bumpSoundSource;

    private bool isAttacking = false;
    private float idleX;
    private Rigidbody2D rb;

    void Awake()
    {
        idleX = transform.position.x;
        rb = GetComponent<Rigidbody2D>();

        // Looking for Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    public void TriggerAttack()
    {
        if (!isAttacking) StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        float targetY = (playerTransform != null) ? playerTransform.position.y : transform.position.y;
        transform.position = new Vector3(idleX, targetY, transform.position.z);

        float targetX = (arenaLeftBoundary != null) ? arenaLeftBoundary.position.x : idleX - 40f;
        Vector3 strikePos = new Vector3(targetX, targetY, transform.position.z);

        yield return StartCoroutine(LerpPosition(transform.position, strikePos, strikeDuration));
        yield return new WaitForSeconds(stayDuration);

        Vector3 returnPos = new Vector3(idleX, transform.position.y, transform.position.z);
        yield return StartCoroutine(LerpPosition(transform.position, returnPos, retractDuration));

        isAttacking = false;
    }

    private IEnumerator LerpPosition(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            // transform.position should be enough for a Trigger?
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }

    // --- TRIGGER ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Checking for Player Tag
        if (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player")))
        {
            if (bumpSoundSource != null) bumpSoundSource.Play();

            // 1. Cause Damage
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("<color=cyan>BouncerFist Trigger: Schaden!</color>");
            }

            // 2. Knockback (=> Rigidbody in Player, "Parent" because "GetComponent only" didn't work)
            Rigidbody2D pRb = other.GetComponentInParent<Rigidbody2D>();
            if (pRb != null)
            {
                pRb.linearVelocity = Vector2.zero;
                pRb.AddForce(new Vector2(-knockbackForce, upwardForce), ForceMode2D.Impulse);
                Debug.Log("<color=cyan>BouncerFist Trigger: Player weggeschubst!</color>");
            }
        }
    }
}
// I currently use an invisible Wall that spawns at the appropriate spot, so the fist flies, does some damage, the player collides and falls down