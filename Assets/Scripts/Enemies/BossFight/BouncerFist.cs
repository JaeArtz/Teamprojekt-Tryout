using UnityEngine;

// --- DIESER TEIL ERZEUGT DEN TEST-BUTTON IM INSPECTOR ---
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(BouncerFist))]
public class BouncerFistEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BouncerFist script = (BouncerFist)target;
        if (Application.isPlaying)
        {
            if (GUILayout.Button("TEST SCHLAG"))
            {
                script.TriggerAttack();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Starte das Spiel, um den Test-Button zu nutzen.", MessageType.Info);
        }
    }
}
#endif
// -------------------------------------------------------

[RequireComponent(typeof(Rigidbody2D))]
public class BouncerFist : MonoBehaviour
{
    [Header("--- Target & Layer Setup ---")]
    public Transform playerTransform;
    public Transform arenaLeftBoundary;

    [Header("--- Movement Settings ---")]
    public float attackSpeed = 30f;
    public float returnSpeed = 10f;

    [Header("--- Combat Settings ---")]
    public int damage = 2;
    public float knockbackForce = 25f;
    public float upwardForce = 6f;

    [Header("--- Audio ---")]
    public AudioSource bumpSoundSource;

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isReturning = false;
    private float idleX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // Wichtig für Kollisionen mit Kinematic Objekten
        rb.useFullKinematicContacts = true;

        idleX = transform.position.x;
    }

    public void TriggerAttack()
    {
        if (isAttacking || isReturning || playerTransform == null) return;

        // Faust auf Player-Höhe setzen
        transform.position = new Vector3(idleX, playerTransform.position.y, 0);
        isAttacking = true;
        Debug.Log("<color=cyan>BouncerFist:</color> Angriff gestartet!");
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            Vector2 nextPos = rb.position + Vector2.left * attackSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);

            // PRÜFUNG: Wenn wir den Grenzpunkt LINKS erreicht/passiert haben
            if (arenaLeftBoundary != null && nextPos.x <= arenaLeftBoundary.position.x)
            {
                Debug.Log("<color=yellow>BouncerFist:</color> Grenze erreicht. Rückzug.");
                StartReturn();
            }
        }
        else if (isReturning)
        {
            Vector2 targetPos = new Vector2(idleX, rb.position.y);
            Vector2 nextPos = Vector2.MoveTowards(rb.position, targetPos, returnSpeed * Time.fixedDeltaTime);
            rb.MovePosition(nextPos);

            if (Vector2.Distance(rb.position, targetPos) < 0.1f)
            {
                isReturning = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (bumpSoundSource != null) bumpSoundSource.Play();

            // Schaden
            var health = collision.gameObject.GetComponentInParent<PlayerHealth>() ?? collision.gameObject.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(damage);

            // Knockback
            Rigidbody2D playerRb = collision.gameObject.GetComponentInParent<Rigidbody2D>() ?? collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(new Vector2(-knockbackForce, upwardForce), ForceMode2D.Impulse);
            }

            StartReturn();
        }
    }

    private void StartReturn()
    {
        isAttacking = false;
        isReturning = true;
    }
}