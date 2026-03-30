using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class BossLimb : MonoBehaviour
{
    [Header("Attack Type")]
    public bool isPlayerTracker = false;
    public Transform playerTransform;

    [Header("Movement Limits (Points)")]
    [Tooltip("Empty Object für die linke Grenze")]
    public Transform minXPoint;
    [Tooltip("Empty Object für die rechte Grenze")]
    public Transform maxXPoint;

    [Header("Visuals & Warning")]
    public GameObject shadowSprite;
    public float warningTime = 1.0f;

    [Header("Movement Settings")]
    public float strikeDuration = 0.15f;
    public float stayDuration = 0.6f;
    public float retractDuration = 1.2f;

    [Header("Leg Specific Settings")]
    public float legRaiseHeight = 8f;
    public float legRaiseDuration = 1.5f;

    [Header("Distance Settings")]
    [Tooltip("Deine Bodenhöhe (94)")]
    public float groundYPosition = 94f;
    [Tooltip("Die Höhe, in der die Fäuste schweben (z.B. 105)")]
    public float attackHeightY = 105f;

    private Collider2D damageCollider;
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        damageCollider = GetComponent<Collider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (damageCollider) damageCollider.enabled = false;
        if (shadowSprite) shadowSprite.SetActive(false);
    }

    public IEnumerator Attack()
    {
        // Sicherheits-Check: Falls der Player im Inspector vergessen wurde, suchen wir ihn über das Tag
        if (isPlayerTracker && playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerTransform = player.transform;
        }

        if (isPlayerTracker) yield return StartCoroutine(FistAttack());
        else yield return StartCoroutine(LegAttack());
    }

    private IEnumerator FistAttack()
    {
        // 1. POSITIONIERUNG: Aktuelle X-Position des Spielers holen
        float targetX = (playerTransform != null) ? playerTransform.position.x : transform.position.x;

        // Begrenzung durch die Empty Objects
        if (minXPoint != null && maxXPoint != null)
        {
            targetX = Mathf.Clamp(targetX, minXPoint.position.x, maxXPoint.position.x);
        }

        // Faust direkt über das Ziel teleportieren
        transform.position = new Vector3(targetX, attackHeightY, transform.position.z);
        Vector3 targetGroundPos = new Vector3(targetX, groundYPosition, transform.position.z);

        // 2. SCHATTEN ZEIGEN
        if (shadowSprite)
        {
            shadowSprite.transform.position = targetGroundPos;
            shadowSprite.SetActive(true);
        }
        yield return new WaitForSeconds(warningTime);

        // 3. EINSCHLAG (STRIKE)
        if (damageCollider) damageCollider.enabled = true;
        yield return LerpPosition(transform.position, targetGroundPos, strikeDuration);

        TriggerShake();

        yield return new WaitForSeconds(stayDuration);
        if (damageCollider) damageCollider.enabled = false;
        if (shadowSprite) shadowSprite.SetActive(false);

        // 4. RÜCKZUG
        yield return LerpPosition(transform.position, new Vector3(transform.position.x, attackHeightY, transform.position.z), retractDuration);
    }

    private IEnumerator LegAttack()
    {
        // Beine starten und enden IMMER bei groundYPosition (94)
        Vector3 floorPos = new Vector3(transform.position.x, groundYPosition, transform.position.z);
        Vector3 raisedPos = new Vector3(transform.position.x, groundYPosition + legRaiseHeight, transform.position.z);

        // Sicherheit: Bein auf den Boden setzen, bevor es hochfährt
        transform.position = floorPos;

        // 1. ANHEBEN
        yield return LerpPosition(floorPos, raisedPos, legRaiseDuration);

        // 2. SCHATTEN & WARNUNG
        if (shadowSprite)
        {
            shadowSprite.transform.position = floorPos;
            shadowSprite.SetActive(true);
        }
        yield return new WaitForSeconds(warningTime);

        // 3. STAMPFEN
        if (damageCollider) damageCollider.enabled = true;
        yield return LerpPosition(raisedPos, floorPos, strikeDuration);

        TriggerShake();

        yield return new WaitForSeconds(stayDuration);
        if (damageCollider) damageCollider.enabled = false;
        if (shadowSprite) shadowSprite.SetActive(false);

        // Sicherheit: Bein fix auf Y = 94 halten
        transform.position = floorPos;
    }

    private void TriggerShake()
    {
        if (impulseSource != null) impulseSource.GenerateImpulse();
    }

    private IEnumerator LerpPosition(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }
}