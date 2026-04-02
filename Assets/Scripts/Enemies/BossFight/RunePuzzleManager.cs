using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class RunePuzzleManager : MonoBehaviour

{

    [Header("--- Rune Puzzle Core ---")]

    [Tooltip("List of IDs that need to be activated.")]

    public List<string> requiredRuneIDs;

    [Tooltip("Reference to all BossRunes in the Area.")]

    public List<RuneAction> allBossRunes;

    private List<RuneAction> activeRunes = new List<RuneAction>();

    private bool _puzzleComplete = false;



    [Header("--- Camera Setup ---")]

    [Tooltip("Cinemachine Virtual Camera.")]

    public CinemachineCamera vcam;

    [Tooltip("The CamTracker-Object you use to have the Cam follow it around.")]

    public Transform cameraFollowTarget;

    private Vector3 originalTargetLocalPos;

    public PlayerController playerController;



    [Header("--- Sequence Waypoints ---")]

    [Tooltip("Object with Reference Position of Camera on RuneStone (Sleeping and Waking Rune).")]

    public Transform runeStoneCamPoint;

    [Tooltip("Object with Reference Position of Cam on SleepingGolem, who then wakes up.")]

    public Transform golemCamPoint;



    [Header("--- Timing Settings ---")]

    [Tooltip("Duration of CamTravel from Player to RuneStone (Sleeping and Waking Rune).")]

    public float durationToRuneStone = 1.5f;

    [Tooltip("Waiting Time before you see the RuneSwap (Glowing switches from Sleeping to WakingRune).")]

    public float delayBeforeRuneSwap = 0.5f;

    [Tooltip("Waiting Time to look at freshly activated WakingRune, before traveling to GolemBoss.")]

    public float durationRuneChangeWait = 2.0f;

    [Tooltip("Duration of CamTravel from RuneStone (Sleeping and Waking Rune) to GolemBoss who wakes up.")]

    public float durationToGolem = 2.5f;

    [Tooltip("Duration you hold Cam on Waking GolemBoss.")]

    public float waitTimeAtGolem = 3.0f;

    [Tooltip("Duration of CamTravel from freshly Risen GolemBoss back to Player.")]

    public float durationBackToPlayer = 1.5f;

    [Tooltip("Movement curve, smooth traveling, for all Cam Movement using movementCurve.")]

    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);



    [Header("--- Visual & Shake Settings ---")]

    [Tooltip("How far does the Camera zoom out/away, while we see the RuneSwap and the GolemBoss waking up?")]

    public float zoomOutSize = 18f;

    [Tooltip("Back to Standard Setting (was initially 10 in Cinemachine Cam).")]

    public float normalZoom = 10f;

    [Tooltip("Intensity of Camera Shake.")]

    public float shakeIntensity = 0.3f;

    [Tooltip("Control when the EarthquakeSound starts: -1 = it starts before Cam arrives at the Golemboss for WakingUpAnimation, 1 = it starts after.")]

    public float earthquakeSoundDelay = 0.2f;



    [Header("--- Audio & Animation ---")]

    [Tooltip("Sound for Earthquake, Waking up GolemBoss.")]

    public AudioSource earthquakeSound;

    [Tooltip("Animator of GolemBoss, waking up.")]

    public Animator golemAnimator;

    [Tooltip("Sound played while Glow switches from SleepingRune to WakingRune.")]

    public AudioSource runeSwapSound;



    [Header("--- RuneStone Visuals ---")]

    [Tooltip("Object for SleepingRune in Scene.")]

    public GameObject sleepingRuneVisual;

    [Tooltip("Object for WakingRune in Scene..")]

    public GameObject wakingRuneVisual;



    [Header("--- Scene Transitions ---")]

    [Tooltip("Container-Object holding all the GolemBossLimbs and things for the FightAnimation.")]

    public GameObject bossFightContainer;

    [Tooltip("Interaction-Script, RuneStone(SleepingRune), exists after Player activated Golem, lets Player deactivate Golem.")]

    public RuneStoneInteraction mainRuneStone;

    [Tooltip("Invisible Walls, keeps Player form walking into Boss Area during Wakeup-Sequence.")]

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



        playerController.SetInputLocked(true);



        // PHASE 1: Preparation

        foreach (GameObject wall in invisibleWalls)

            if (wall != null) wall.SetActive(true);



        if (cameraFollowTarget != null) originalTargetLocalPos = cameraFollowTarget.localPosition;



        // PHASE 2: CamTravel to RuneStone (Sleeping and Waking Rune)

        yield return StartCoroutine(LerpCamera(cameraFollowTarget.position, runeStoneCamPoint.position, durationToRuneStone));



        // PHASE 3: Rune Swap at RuneStone

        yield return new WaitForSeconds(delayBeforeRuneSwap);



        if (runeSwapSound != null) runeSwapSound.Play();

        if (sleepingRuneVisual != null) sleepingRuneVisual.SetActive(false);

        if (wakingRuneVisual != null) wakingRuneVisual.SetActive(true);



        yield return new WaitForSeconds(durationRuneChangeWait);



        // enables +1 or -1 for shifting start of Earthquake Sound 

        float totalWaitUntilSound = durationToGolem + earthquakeSoundDelay;

        StartCoroutine(PlaySoundDelayed(totalWaitUntilSound));



        // PHASE 4: CamTravel to Waking GolemBoss

        yield return StartCoroutine(LerpCamera(runeStoneCamPoint.position, golemCamPoint.position, durationToGolem));



        // PHASE 5: Earthquake and Waking-Animation

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



        // PHASE 6: CamTravel back to Player

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



        playerController.SetInputLocked(false);

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

        // Clamp in case the SoundDelay was beyond the whole playtime of sequence         

        if (delay < 0) delay = 0;



        yield return new WaitForSeconds(delay);

        if (earthquakeSound != null) earthquakeSound.Play();

    }

}