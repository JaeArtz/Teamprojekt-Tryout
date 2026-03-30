using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class BossFist : MonoBehaviour
{
    [Header("Targeting")]
    public Transform playerTransform;
    public Transform minXLimit;
    public Transform maxXLimit;

    [Header("Heights")]
    public float groundY = 94f;
    public float waitingHeightY = 115f;

    [Header("Settings")]
    public float strikeDuration = 0.15f;
    public float stayDuration = 0.6f;
    public float retractDuration = 1.2f;
    public float warningTime = 0.8f;
    public GameObject shadowSprite;

    private Collider2D damageCollider;
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        damageCollider = GetComponent<Collider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (damageCollider) damageCollider.enabled = false;
        if (shadowSprite) shadowSprite.SetActive(false);

        // Startposition in der Luft
        transform.position = new Vector3(transform.position.x, waitingHeightY, transform.position.z);
    }

    public IEnumerator Attack()
    {
        float targetX = (playerTransform != null) ? playerTransform.position.x : transform.position.x;

        // --- DIE BEGRENZUNG (CLAMP) ---
        if (minXLimit != null && maxXLimit != null)
        {
            // Wir ermitteln die absolute linke und rechte Grenze aus den beiden Punkten
            float leftBoundary = Mathf.Min(minXLimit.position.x, maxXLimit.position.x);
            float rightBoundary = Mathf.Max(minXLimit.position.x, maxXLimit.position.x);

            // targetX wird hart zwischen diese beiden Werte gezwungen
            targetX = Mathf.Clamp(targetX, leftBoundary, rightBoundary);
        }

        // Position für den Angriff fixieren
        transform.position = new Vector3(targetX, waitingHeightY, transform.position.z);
        Vector3 targetGround = new Vector3(targetX, groundY, transform.position.z);

        // Schatten anzeigen
        if (shadowSprite)
        {
            shadowSprite.transform.position = targetGround;
            shadowSprite.SetActive(true);
        }

        yield return new WaitForSeconds(warningTime);

        // Schlag nach unten
        if (damageCollider) damageCollider.enabled = true;
        yield return StartCoroutine(LerpPosition(transform.position, targetGround, strikeDuration));

        // Effekt & Sound
        if (impulseSource) impulseSource.GenerateImpulse();
        if (BossCombatManager.Instance != null)
        {
            BossCombatManager.Instance.TriggerFistSound();
        }

        yield return new WaitForSeconds(stayDuration);

        // Rückzug
        if (damageCollider) damageCollider.enabled = false;
        if (shadowSprite) shadowSprite.SetActive(false);

        yield return StartCoroutine(LerpPosition(transform.position, new Vector3(transform.position.x, waitingHeightY, transform.position.z), retractDuration));
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