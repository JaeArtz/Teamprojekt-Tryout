using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class BossLegMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float raiseHeight = 7f;
    public float raiseDuration = 0.6f;
    public float strikeDuration = 0.15f;
    public float stayDuration = 0.6f;
    public float warningTime = 3f;

    [Header("Visuals & Effects")]
    public GameObject shadowSprite;
    public CinemachineImpulseSource impulseSource;

    [Header("Audio Sources")]
    public AudioSource bumpSource;   // Hier BossPunch rein
    public AudioSource rumbleSource; // Hier BossStomp rein

    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
        if (impulseSource == null) impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // DIESE Methode wird vom BossCombatManagerTest aufgerufen
    public IEnumerator Attack()
    {
        if (shadowSprite) shadowSprite.SetActive(true);
        yield return new WaitForSeconds(warningTime);

        // Hochbewegen
        yield return StartCoroutine(MoveLeg(startPosition + Vector3.up * raiseHeight, raiseDuration));

        // Runterschlagen
        yield return StartCoroutine(MoveLeg(startPosition, strikeDuration));

        // EFFEKTE AUSLÖSEN (Shake, Sound & Schockwellen via Manager)
        TriggerImpactEffects();

        if (shadowSprite) shadowSprite.SetActive(false);
        yield return new WaitForSeconds(stayDuration);
    }

    private void TriggerImpactEffects()
    {
        if (impulseSource != null) impulseSource.GenerateImpulse();
        if (bumpSource != null) bumpSource.Play();
        if (rumbleSource != null) rumbleSource.Play();

        // Hier wird der Manager angewiesen, die Schockwellen zu spawnen
        if (BossCombatManagerTest.Instance != null)
        {
            BossCombatManagerTest.Instance.TriggerStompEffects(transform.position);
        }
    }

    IEnumerator MoveLeg(Vector3 target, float duration)
    {
        float elapsed = 0;
        Vector3 from = transform.position;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }
}