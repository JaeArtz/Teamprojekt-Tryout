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

        // Startposition "in Air"
        transform.position = new Vector3(transform.position.x, waitingHeightY, transform.position.z);
    }

    public IEnumerator Attack()
    {
        float targetX = (playerTransform != null) ? playerTransform.position.x : transform.position.x;

        // --- CLAMP ---
        if (minXLimit != null && maxXLimit != null)
        {
            // getting left and right boundary for "free movement", and tracking of player
            // Left Fist os only supposed to punch down on left half, righ Fist on right side of Golem
            float leftBoundary = Mathf.Min(minXLimit.position.x, maxXLimit.position.x);
            float rightBoundary = Mathf.Max(minXLimit.position.x, maxXLimit.position.x);

            // targetX = X position of Player at time of "Targeting" (Shadow)
            targetX = Mathf.Clamp(targetX, leftBoundary, rightBoundary);
        }

        //fix position for attack, using the current targetX
        transform.position = new Vector3(targetX, waitingHeightY, transform.position.z);
        Vector3 targetGround = new Vector3(targetX, groundY, transform.position.z);

        // CAST SHADOW
        if (shadowSprite)
        {
            shadowSprite.transform.position = targetGround;
            shadowSprite.SetActive(true);
        }

        yield return new WaitForSeconds(warningTime);

        // PUNCH DOWN
        if (damageCollider) damageCollider.enabled = true;
        yield return StartCoroutine(LerpPosition(transform.position, targetGround, strikeDuration));

        // EFFECT & SOUND
        if (impulseSource) impulseSource.GenerateImpulse();
        if (BossCombatManager.Instance != null)
        {
            BossCombatManager.Instance.TriggerFistSound();
        }

        yield return new WaitForSeconds(stayDuration);

        // RETREAT
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