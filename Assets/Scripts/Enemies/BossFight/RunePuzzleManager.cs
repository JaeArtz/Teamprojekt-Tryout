using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class RunePuzzleManager : MonoBehaviour
{
    [Header("Rune Settings")]
    public List<string> requiredRuneIDs;
    public List<RuneAction> allBossRunes;
    private List<RuneAction> activeRunes = new List<RuneAction>();
    private bool _puzzleComplete = false;

    [Header("Sequence: Camera & Walls")]
    public float zoomOutSize = 18f;
    public float panRightAmount = 12f;
    public float panUpAmount = 5f;
    public float waitTimeRight = 3f;
    public float cameraSpeed = 1.5f;
    public float normalZoom = 10f;
    public float shakeIntensity = 0.3f;

    [Header("Sequence: Sound Timing")]
    [Tooltip("Verzögerung in Sekunden, bis der Erdbeben-Sound startet")]
    public float soundDelay = 0.5f; // controls start of EarthQuake Sound

    [Header("Physical Barriers")]
    public List<GameObject> invisibleWalls; // keeps Player in Rune-Zone during Boss-Animation

    [Header("References")]
    public Animator golemAnimator;
    public AudioSource earthquakeSound;
    public CinemachineCamera vcam;
    public Transform cameraFollowTarget;

    private Vector3 originalTargetLocalPos;

    public void RegisterRuneActivation(string id, RuneAction script, TriggerInfoBundle ctx)
    {
        if (requiredRuneIDs.Contains(id) && !activeRunes.Contains(script))
        {
            activeRunes.Add(script);
            if (activeRunes.Count >= requiredRuneIDs.Count)
                StartCoroutine(StartSequence(ctx));
        }
    }

    public void ResetPuzzle()
    {
        if (_puzzleComplete) return;
        activeRunes.Clear();
        foreach (var rune in allBossRunes)
            if (rune != null) rune.Deactivate();
    }

    private IEnumerator StartSequence(TriggerInfoBundle ctx)
    {
        _puzzleComplete = true;

        // --- 1. ACTIVATE WALLS (to trap in Player) ---
        foreach (GameObject wall in invisibleWalls)
        {
            if (wall != null) wall.SetActive(true);
        }

        // PLAY SOUND, delay optional        
        StartCoroutine(PlaySoundDelayed(soundDelay));

        if (cameraFollowTarget != null) originalTargetLocalPos = cameraFollowTarget.localPosition;

        // --- 2. CAM TRANSITION ---
        Vector3 targetPanPos = originalTargetLocalPos + new Vector3(panRightAmount, panUpAmount, 0);
        while (Mathf.Abs(vcam.Lens.OrthographicSize - zoomOutSize) > 0.1f ||
               Vector3.Distance(cameraFollowTarget.localPosition, targetPanPos) > 0.1f)
        {
            vcam.Lens.OrthographicSize = Mathf.MoveTowards(vcam.Lens.OrthographicSize, zoomOutSize, cameraSpeed * Time.deltaTime);
            if (cameraFollowTarget != null)
                cameraFollowTarget.localPosition = Vector3.MoveTowards(cameraFollowTarget.localPosition, targetPanPos, cameraSpeed * Time.deltaTime);

            // --- EARTHQUAKE VISUAL ---
            Vector3 shakeOffset = Random.insideUnitSphere * (shakeIntensity * 0.5f);
            shakeOffset.z = 0;
            cameraFollowTarget.localPosition += shakeOffset;

            yield return null;
        }

        // --- 3. MAIN-QUAKE ---        
        if (golemAnimator != null) golemAnimator.SetTrigger("Awake");

        float shakeTimer = 2.0f; // Short After-Shaking
        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (cameraFollowTarget != null)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
                shakeOffset.z = 0;
                cameraFollowTarget.localPosition = targetPanPos + shakeOffset;
            }
            yield return null;
        }
        cameraFollowTarget.localPosition = targetPanPos;

        yield return new WaitForSeconds(waitTimeRight);

        // --- 4. CAM BACK ---
        while (Mathf.Abs(vcam.Lens.OrthographicSize - normalZoom) > 0.1f ||
               Vector3.Distance(cameraFollowTarget.localPosition, originalTargetLocalPos) > 0.1f)
        {
            vcam.Lens.OrthographicSize = Mathf.MoveTowards(vcam.Lens.OrthographicSize, normalZoom, cameraSpeed * Time.deltaTime);
            if (cameraFollowTarget != null)
                cameraFollowTarget.localPosition = Vector3.MoveTowards(cameraFollowTarget.localPosition, originalTargetLocalPos, cameraSpeed * Time.deltaTime);
            yield return null;
        }

        // --- 5. DEACTIVATE WALLS (frees player again for movement into Boss area) ---
        foreach (GameObject wall in invisibleWalls)
        {
            if (wall != null) wall.SetActive(false);
        }
    }

    // HelperFunction for Delay
    private IEnumerator PlaySoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (earthquakeSound != null) earthquakeSound.Play();
    }
}