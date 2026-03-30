using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class BossLimb : MonoBehaviour
{
    [Header("Attack Type")]
    public bool isPlayerTracker = false;
    public Transform playerTransform;

    [Header("Movement Limits (Points)")]
    [Tooltip("Insert Empty Object for Left Border,\"Movement Cap\"")]
    public Transform minXPoint;
    [Tooltip("Insert Empty Object for Right Border,\"Movement Cap\"")]
    public Transform maxXPoint;

    [Header("Visuals & Warning")]
    public GameObject shadowSprite;
    [Tooltip("Time in which Shadow exists, before the Fist hits the Spot")]
    public float warningTime = 1.0f;

    [Header("Movement Settings")]
    public float strikeDuration = 0.15f;
    public float stayDuration = 0.6f;
    public float retractDuration = 1.2f;

    [Header("Leg Specific Settings")]
    public float legRaiseHeight = 8f;
    public float legRaiseDuration = 1.5f;

    [Header("Distance Settings")]
    [Tooltip("My Chosen Height for the Object thats attacking (currently 94)")]
    public float groundYPosition = 94f;
    [Tooltip("Height from which attack is started")]
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
        // Security check, we really want the Player (Player Tag)
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
        // 1. POSITIONING: takes x-coordinate of Player
        float targetX = (playerTransform != null) ? playerTransform.position.x : transform.position.x;

        // Sets "Border", limit of maximum "freedom of movement"
        if (minXPoint != null && maxXPoint != null)
        {
            targetX = Mathf.Clamp(targetX, minXPoint.position.x, maxXPoint.position.x);
        }

        // Teleports Fist directly above position of Player, for attack
        transform.position = new Vector3(targetX, attackHeightY, transform.position.z);
        Vector3 targetGroundPos = new Vector3(targetX, groundYPosition, transform.position.z);

        // 2. CAST SHADOW
        if (shadowSprite)
        {
            shadowSprite.transform.position = targetGroundPos;
            shadowSprite.SetActive(true);
        }
        yield return new WaitForSeconds(warningTime);

        // 3. STRIKE
        if (damageCollider) damageCollider.enabled = true;
        yield return LerpPosition(transform.position, targetGroundPos, strikeDuration);

        TriggerShake();

        yield return new WaitForSeconds(stayDuration);
        if (damageCollider) damageCollider.enabled = false;
        if (shadowSprite) shadowSprite.SetActive(false);

        // 4. RETREAT
        yield return LerpPosition(transform.position, new Vector3(transform.position.x, attackHeightY, transform.position.z), retractDuration);
    }

    private IEnumerator LegAttack()
    {
        // Legs always start at, and retreat to same coordinate/ "Height"
        Vector3 floorPos = new Vector3(transform.position.x, groundYPosition, transform.position.z);
        Vector3 raisedPos = new Vector3(transform.position.x, groundYPosition + legRaiseHeight, transform.position.z);

        // Put Leg on Ground before raising it
        transform.position = floorPos;

        // 1. LIFT LEG
        yield return LerpPosition(floorPos, raisedPos, legRaiseDuration);

        // 2. CAST SHADOW // was taken off from foot, looked weird
        if (shadowSprite)
        {
            shadowSprite.transform.position = floorPos;
            shadowSprite.SetActive(true);
        }
        yield return new WaitForSeconds(warningTime);

        // 3. STOMP
        if (damageCollider) damageCollider.enabled = true;
        yield return LerpPosition(raisedPos, floorPos, strikeDuration);

        TriggerShake();

        yield return new WaitForSeconds(stayDuration);
        if (damageCollider) damageCollider.enabled = false;
        if (shadowSprite) shadowSprite.SetActive(false);

        // Keeps Leg fixed to requested Y-Position (Ground-Level)
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