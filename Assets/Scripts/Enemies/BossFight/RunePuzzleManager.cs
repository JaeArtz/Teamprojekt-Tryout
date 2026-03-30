using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class RunePuzzleManager : MonoBehaviour
{
    [Header("--- Rune Puzzle Core ---")]
    [Tooltip("Liste der IDs, die für das Rätsel aktiviert werden müssen.")]
    public List<string> requiredRuneIDs;
    [Tooltip("Referenz auf alle Runen-Skripte in der Arena.")]
    public List<RuneAction> allBossRunes;
    private List<RuneAction> activeRunes = new List<RuneAction>();
    private bool _puzzleComplete = false;

    [Header("--- Camera Setup ---")]
    [Tooltip("Die Cinemachine Virtual Camera.")]
    public CinemachineCamera vcam;
    [Tooltip("Das Target-Objekt (Tracker), dem die Kamera folgt.")]
    public Transform cameraFollowTarget;
    private Vector3 originalTargetLocalPos;

    [Header("--- Sequence Waypoints ---")]
    [Tooltip("Position der Kamera am Runen-Wandbild.")]
    public Transform runeStoneCamPoint;
    [Tooltip("Position der Kamera am schlafenden Golem.")]
    public Transform golemCamPoint;

    [Header("--- Timing Settings ---")]
    [Tooltip("Dauer der Kamerafahrt vom Spieler zum Wandbild.")]
    public float durationToRuneStone = 1.5f;
    [Tooltip("Wartezeit am Wandbild, BEVOR der Runenwechsel optisch passiert.")]
    public float delayBeforeRuneSwap = 0.5f;
    [Tooltip("Wartezeit am Wandbild NACHDEM die Rune umgesprungen ist.")]
    public float durationRuneChangeWait = 2.0f;
    [Tooltip("Dauer der Kamerafahrt vom Wandbild zum Golem.")]
    public float durationToGolem = 2.5f;
    [Tooltip("Dauer, die die Kamera beim erwachenden Golem verweilt.")]
    public float waitTimeAtGolem = 3.0f;
    [Tooltip("Dauer der Rückfahrt zum Spieler.")]
    public float durationBackToPlayer = 1.5f;
    [Tooltip("Bewegungskurve für alle Kamerafahrten (Einfrieren/Beschleunigen).")]
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("--- Visual & Shake Settings ---")]
    [Tooltip("Zoom-Größe während der Kamerafahrt (höherer Wert = weiter weg).")]
    public float zoomOutSize = 18f;
    [Tooltip("Standard-Zoom-Größe beim Spieler.")]
    public float normalZoom = 10f;
    [Tooltip("Intensität des Kamerazitterns beim Erdbeben.")]
    public float shakeIntensity = 0.3f;
    [Tooltip("Verzögerung relativ zur Ankunft am Golem. Negative Werte lassen den Sound SCHON WÄHREND der Anfahrt starten.")]
    public float earthquakeSoundDelay = 0.2f;

    [Header("--- Audio & Animation ---")]
    [Tooltip("Sound für das Erdbeben/Erwachen.")]
    public AudioSource earthquakeSound;
    [Tooltip("Animator des Golems (für den Awake-Trigger).")]
    public Animator golemAnimator;
    [Tooltip("Sound, der beim Umspringen der Runen am Wandbild spielt.")]
    public AudioSource runeSwapSound;

    [Header("--- RuneStone Visuals ---")]
    [Tooltip("Das Objekt für das 'Schlaf'-Symbol am Stein.")]
    public GameObject sleepingRuneVisual;
    [Tooltip("Das Objekt für das 'Wach'-Symbol am Stein.")]
    public GameObject wakingRuneVisual;

    [Header("--- Scene Transitions ---")]
    [Tooltip("Der Container, der die Boss-Gliedmaßen (Kampf-Logik) aktiviert.")]
    public GameObject bossFightContainer;
    [Tooltip("Interaktions-Skript am Wandbild (wird nach Kampf aktiviert).")]
    public RuneStoneInteraction mainRuneStone;
    [Tooltip("Unsichtbare Wände, die den Spieler während der Sequenz einsperren.")]
    public List<GameObject> invisibleWalls;

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

        // PHASE 1: VORBEREITUNG
        foreach (GameObject wall in invisibleWalls)
            if (wall != null) wall.SetActive(true);

        if (cameraFollowTarget != null) originalTargetLocalPos = cameraFollowTarget.localPosition;

        // PHASE 2: FAHRT ZUM WANDBILD
        yield return StartCoroutine(LerpCamera(cameraFollowTarget.position, runeStoneCamPoint.position, durationToRuneStone));

        // PHASE 3: RUNEN-WECHSEL AM STEIN
        yield return new WaitForSeconds(delayBeforeRuneSwap);

        if (runeSwapSound != null) runeSwapSound.Play();
        if (sleepingRuneVisual != null) sleepingRuneVisual.SetActive(false);
        if (wakingRuneVisual != null) wakingRuneVisual.SetActive(true);

        yield return new WaitForSeconds(durationRuneChangeWait);

        // --- NEUE SOUND-LOGIK ---
        // Wir starten den Sound-Timer BEVOR die Fahrt zum Golem beginnt.
        // Fahrtzeit + Delay ergibt den exakten Zeitpunkt relativ zur Ankunft.
        float totalWaitUntilSound = durationToGolem + earthquakeSoundDelay;
        StartCoroutine(PlaySoundDelayed(totalWaitUntilSound));

        // PHASE 4: WEITERFAHRT ZUM GOLEM
        yield return StartCoroutine(LerpCamera(runeStoneCamPoint.position, golemCamPoint.position, durationToGolem));

        // PHASE 5: ERWACHEN (BEBEN & ANIMATION)
        if (golemAnimator != null) golemAnimator.SetTrigger("Awake");

        float shakeTimer = 2.0f;
        Vector3 golemFinalPos = golemCamPoint.position;
        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            cameraFollowTarget.position = golemFinalPos + (Vector3)(Random.insideUnitCircle * shakeIntensity);
            yield return null;
        }
        cameraFollowTarget.position = golemFinalPos;

        yield return new WaitForSeconds(waitTimeAtGolem);

        // PHASE 6: RÜCKFAHRT ZUM PLAYER
        float elapsed = 0f;
        float returnStartZoom = vcam.Lens.OrthographicSize;
        while (elapsed < durationBackToPlayer)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / durationBackToPlayer);
            float curveT = movementCurve.Evaluate(t);

            cameraFollowTarget.localPosition = Vector3.Lerp(cameraFollowTarget.localPosition, originalTargetLocalPos, curveT);
            vcam.Lens.OrthographicSize = Mathf.Lerp(returnStartZoom, normalZoom, curveT);
            yield return null;
        }

        vcam.Lens.OrthographicSize = normalZoom;
        cameraFollowTarget.localPosition = originalTargetLocalPos;

        if (golemAnimator != null) golemAnimator.gameObject.SetActive(false);
        if (bossFightContainer != null) bossFightContainer.SetActive(true);

        if (mainRuneStone != null) mainRuneStone.EnableInteraction();

        foreach (GameObject wall in invisibleWalls)
            if (wall != null) wall.SetActive(false);
    }

    private IEnumerator LerpCamera(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        float startZoom = vcam.Lens.OrthographicSize;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveT = movementCurve.Evaluate(t);

            cameraFollowTarget.position = Vector3.Lerp(startPos, endPos, curveT);
            vcam.Lens.OrthographicSize = Mathf.Lerp(startZoom, zoomOutSize, curveT);

            Vector3 shake = (Vector3)(Random.insideUnitCircle * (shakeIntensity * 0.1f));
            cameraFollowTarget.position += shake;

            yield return null;
        }
    }

    private IEnumerator PlaySoundDelayed(float delay)
    {
        // Falls der Delay negativ war und die Fahrtzeit übersteigt, 
        // wird er hier auf 0 geklemmt, damit kein Fehler auftritt.
        if (delay < 0) delay = 0;

        yield return new WaitForSeconds(delay);
        if (earthquakeSound != null) earthquakeSound.Play();
    }
}