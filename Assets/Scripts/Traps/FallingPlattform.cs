using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private float destroyDelay = 2f; // Zeit bis zum Verschwinden
    [SerializeField] private float respawnDelay = 3f; // Zeit bis zum Wiederauftauchen
    [SerializeField] private float gravityScale = 1f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D platformCollider;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool falling = false;

    private void Awake()
    {
        // Startwerte speichern
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (falling) return;

        if (collision.transform.CompareTag("Player"))
        {
            StartCoroutine(StartFall());
        }
    }

    private IEnumerator StartFall()
    {
        falling = true;
        yield return new WaitForSeconds(fallDelay);

        // Physik aktivieren
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;

        // Warten bis die Plattform "zerstört" werden soll
        yield return new WaitForSeconds(destroyDelay);

        // Deaktivieren
        SetPlatformState(false);

        // Respawn einleiten
        yield return new WaitForSeconds(respawnDelay);
        ResetPlatform();
    }

    private void ResetPlatform()
    {
        // Zurücksetzen an den Start
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        // Physik zurücksetzen
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Wieder aktivieren
        SetPlatformState(true);
        falling = false;
    }

    private void SetPlatformState(bool active)
    {
        // Visuell und physikalisch an/ausschalten
        spriteRenderer.enabled = active;
        platformCollider.enabled = active;
    }
}