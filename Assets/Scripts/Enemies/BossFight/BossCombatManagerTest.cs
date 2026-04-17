using UnityEngine;
using System.Collections;

public class BossCombatManagerTest : MonoBehaviour
{
    public static BossCombatManagerTest Instance;

    [Header("Limb References")]
    public BossFist leftFist;
    public BossFist rightFist;
    public BossLegMovement leftLeg;
    public BossLegMovement rightLeg;

    [Header("Player & Arena Tracking")]
    public Transform playerTransform;
    public Transform centerPoint;
    public float groundY = 94f;

    [Header("Rhythm Settings")]
    public float pauseBetweenAttacks = 0.8f;
    public float pauseBetweenCycles = 2.0f;

    private bool playerIsInRange = false;
    private bool bossIsRunning = false;

    [Header("Effects")]
    [SerializeField] private AudioSource rumbleSource;
    [SerializeField] private GameObject shockwavePrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Diese Methode wird vom Bein aufgerufen, um die Wellen zu erzeugen
    public void TriggerStompEffects(Vector3 spawnPos)
    {
        Vector3 groundSpawnPos = new Vector3(spawnPos.x, groundY, spawnPos.z);
        if (rumbleSource != null && rumbleSource.clip != null)
            rumbleSource.PlayOneShot(rumbleSource.clip);

        if (shockwavePrefab != null)
        {
            GameObject waveR = Instantiate(shockwavePrefab, groundSpawnPos, Quaternion.identity);
            if (waveR.GetComponent<Shockwave>() != null) waveR.GetComponent<Shockwave>().Setup(1);

            GameObject waveL = Instantiate(shockwavePrefab, groundSpawnPos, Quaternion.identity);
            if (waveL.GetComponent<Shockwave>() != null) waveL.GetComponent<Shockwave>().Setup(-1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            if (!bossIsRunning) StartCoroutine(BossRhythmLoop());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerIsInRange = false;
    }

    private IEnumerator BossRhythmLoop()
    {
        bossIsRunning = true;
        while (playerIsInRange)
        {
            // --- ORIGINAL RHYTHMUS ---
            yield return StartCoroutine(GetActiveFist().Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);
            yield return StartCoroutine(GetActiveFist().Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);

            if (rightLeg != null) yield return StartCoroutine(rightLeg.Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);
            if (leftLeg != null) yield return StartCoroutine(leftLeg.Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);

            yield return StartCoroutine(GetActiveFist().Attack());
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(GetActiveFist().Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);

            if (leftLeg != null) yield return StartCoroutine(leftLeg.Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);
            if (rightLeg != null) yield return StartCoroutine(rightLeg.Attack());

            yield return new WaitForSeconds(pauseBetweenCycles);
        }
        bossIsRunning = false;
    }

    private BossFist GetActiveFist()
    {
        if (playerTransform == null) return leftFist;
        float centerX = (centerPoint != null) ? centerPoint.position.x : transform.position.x;
        return (playerTransform.position.x < centerX) ? leftFist : rightFist;
    }
}