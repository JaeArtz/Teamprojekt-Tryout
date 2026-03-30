using UnityEngine;
using System.Collections;

public class BossCombatManager : MonoBehaviour
{
    public static BossCombatManager Instance;

    [Header("Limb References")]
    public BossFist leftFist;
    public BossFist rightFist;
    public BossLeg leftLeg;
    public BossLeg rightLeg;

    [Header("Player & Arena Tracking")]
    public Transform playerTransform;
    [Tooltip("Zieh hier dein 'Arena_Limit_Center' Objekt rein")]
    public Transform centerPoint;
    public float groundY = 94f; // y-coordinate of Ground

    [Header("Rhythm Settings")]
    public float pauseBetweenAttacks = 0.8f;
    public float pauseBetweenCycles = 2.0f;

    private bool playerIsInRange = false;
    private bool bossIsRunning = false;

    [Header("Zentrale Effekte")]
    [SerializeField] private AudioSource bumpSource;
    [SerializeField] private AudioSource rumbleSource;
    [SerializeField] private GameObject shockwavePrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // --- LEG (Sound + Shockwave) ---
    public void TriggerStompEffects(Vector3 spawnPos)
    {
        // brings everything to "Ground-Level" (y coordinate, height of Ground)
        Vector3 groundSpawnPos = new Vector3(spawnPos.x, groundY, spawnPos.z);

        // 1. Sound
        if (rumbleSource != null && rumbleSource.clip != null)
            rumbleSource.PlayOneShot(rumbleSource.clip);

        // 2. Shockwave
        if (shockwavePrefab != null)
        {
            // Right
            GameObject waveR = Instantiate(shockwavePrefab, groundSpawnPos, Quaternion.identity);
            var sR = waveR.GetComponent<Shockwave>();
            if (sR != null) sR.Setup(1);

            // Left
            GameObject waveL = Instantiate(shockwavePrefab, groundSpawnPos, Quaternion.identity);
            var sL = waveL.GetComponent<Shockwave>();
            if (sL != null) sL.Setup(-1);
        }
    }

    // --- FÜR DIE FAUST (Nur Sound) ---
    public void TriggerFistSound()
    {
        if (rumbleSource != null && rumbleSource.clip != null)
        {
            rumbleSource.PlayOneShot(rumbleSource.clip);
        }
    }

    #region Trigger & Loop (Unverändert)
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
            yield return StartCoroutine(GetActiveFist().Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);
            yield return StartCoroutine(GetActiveFist().Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);

            yield return StartCoroutine(rightLeg.Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);
            yield return StartCoroutine(leftLeg.Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);

            yield return StartCoroutine(GetActiveFist().Attack());
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(GetActiveFist().Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);

            yield return StartCoroutine(leftLeg.Attack());
            yield return new WaitForSeconds(pauseBetweenAttacks);
            yield return StartCoroutine(rightLeg.Attack());

            yield return new WaitForSeconds(pauseBetweenCycles);
        }

        bossIsRunning = false;
    }

    private BossFist GetActiveFist()
    {
        if (playerTransform == null) return leftFist;
        float centerX = (centerPoint != null) ? centerPoint.position.x : transform.position.x;
        if (playerTransform.position.x < centerX)
            return leftFist;
        else
            return rightFist;
    }
    #endregion
}