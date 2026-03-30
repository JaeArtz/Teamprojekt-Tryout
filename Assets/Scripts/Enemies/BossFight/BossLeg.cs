using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class BossLeg : MonoBehaviour
{
    [Header("Collider Setup")]
    [Tooltip("Der Polygon Collider mit 'Is Trigger = ON'")]
    public Collider2D damageTrigger;
    [Tooltip("Der Box Collider mit 'Is Trigger = OFF'")]
    public Collider2D solidWallCollider;

    [Header("Movement Settings")]
    public float raiseHeight = 8f;
    public float raiseDuration = 1.5f;
    public float strikeDuration = 0.15f;
    public float stayDuration = 0.6f;

    [Header("Visuals")]
    public GameObject shadowSprite;
    public float warningTime = 1.0f;

    private Vector3 groundPos;
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        groundPos = transform.position;
        impulseSource = GetComponent<CinemachineImpulseSource>();

        // Initial-Zustand:
        if (damageTrigger) damageTrigger.enabled = false; // Kein Schaden im Stand
        if (solidWallCollider) solidWallCollider.enabled = true; // Solide Wand im Stand
        if (shadowSprite) shadowSprite.SetActive(false);
    }

    public IEnumerator Attack()
    {
        Vector3 raisedPos = new Vector3(groundPos.x, groundPos.y + raiseHeight, groundPos.z);

        // 1. ANHEBEN
        if (damageTrigger) damageTrigger.enabled = false;
        yield return StartCoroutine(LerpPosition(groundPos, raisedPos, raiseDuration));

        // 2. WARNUNG
        if (shadowSprite)
        {
            shadowSprite.transform.position = groundPos;
            shadowSprite.SetActive(true);
        }
        yield return new WaitForSeconds(warningTime);

        // 3. STAMPFEN (Aktion beginnt)
        // Schaden EIN, Wand AUS (damit der Player nicht unter das Bein teleportiert wird)
        if (damageTrigger) damageTrigger.enabled = false;
        if (solidWallCollider) solidWallCollider.enabled = true;


        yield return StartCoroutine(LerpPosition(raisedPos, groundPos, strikeDuration));

        // 4. AUFPRALL
        if (impulseSource) impulseSource.GenerateImpulse();

        // --- NEU: RUFT SOUND UND WELLEN IM MANAGER AUF ---
        if (BossCombatManager.Instance != null)
        {
            BossCombatManager.Instance.TriggerStompEffects(transform.position);
        }
        // ------------------------------------------------

        // Wand sofort wieder an, Schaden kurz danach aus
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