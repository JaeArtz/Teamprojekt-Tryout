using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class BossLeg : MonoBehaviour
{
    [Header("Collider Setup")]
    [Tooltip("Polygon Collider with 'Is Trigger = ON'")]
    public Collider2D damageTrigger;
    [Tooltip("Box Collider with 'Is Trigger = OFF'")]
    public Collider2D solidWallCollider;

    [Header("Movement Settings for Leg")]
    public float raiseHeight = 8f;
    public float raiseDuration = 1.5f;
    public float strikeDuration = 0.15f;
    public float stayDuration = 0.6f;

    [Header("Visuals")]
    public GameObject shadowSprite;
    public float warningTime = 1.0f;

    private Vector3 groundPos;
    private CinemachineImpulseSource impulseSource; // for "shaking" of cam on Stomp

    void Awake()
    {
        groundPos = transform.position;
        impulseSource = GetComponent<CinemachineImpulseSource>();

        // Initial State:
        if (damageTrigger) damageTrigger.enabled = false; // no damage dealt when Leg ist standing still
        if (solidWallCollider) solidWallCollider.enabled = true; // "solid wall" while standing
        if (shadowSprite) shadowSprite.SetActive(false);
    }

    public IEnumerator Attack()
    {
        Vector3 raisedPos = new Vector3(groundPos.x, groundPos.y + raiseHeight, groundPos.z);

        // 1. LIFT Foot
        if (damageTrigger) damageTrigger.enabled = false;
        yield return StartCoroutine(LerpPosition(groundPos, raisedPos, raiseDuration));

        // 2. "WARNING"
        if (shadowSprite)
        {
            shadowSprite.transform.position = groundPos;
            shadowSprite.SetActive(true);
        }
        yield return new WaitForSeconds(warningTime);

        // 3. STOMP
        // Damage ON, Wall OFF
        if (damageTrigger) damageTrigger.enabled = true;
        Debug.Log($"STOMP - damageTrigger enabled: {damageTrigger.enabled}, Object: {damageTrigger.gameObject.name}");
        if (solidWallCollider) solidWallCollider.enabled = false;


        yield return StartCoroutine(LerpPosition(raisedPos, groundPos, strikeDuration));

        // 4. IMPACT
        if (impulseSource) impulseSource.GenerateImpulse();

        // --- SOUND AND SHOCKWAVES ---
        if (BossCombatManager.Instance != null)
        {
            BossCombatManager.Instance.TriggerStompEffects(transform.position);
        }
        // ------------------------------------------------

        // WALL ON, shortly after DAMAGE OFF
        if (solidWallCollider) solidWallCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        if (damageTrigger) damageTrigger.enabled = false;

        yield return new WaitForSeconds(stayDuration - 0.1f);

        // 5. RESET
        if (shadowSprite) shadowSprite.SetActive(false);
        transform.position = groundPos;
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