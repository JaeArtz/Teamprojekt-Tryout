using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("Target Roots (exactly these two subtrees are scanned)")]
    public Transform windGrassObjects;
    public Transform windGrassObjectsSlowSway;
      
    [Header("Targets (auto-filled)")]
    [Tooltip("All SpriteRenderers that use the WindEffect shader.")]
    public SpriteRenderer[] targets;

    [Header("Shader Properties (Reference names)")]
    [Tooltip("Shader Graph reference name for influence/amplitude.")]
    public string influenceProperty = "_Externalinfluence";

    [Tooltip("Shader Graph reference name for wind speed.")]
    public string speedProperty = "_WindSpeed";

    [Header("Baseline")]
    public float baseInfluence = 0.05f;
    public float baseSpeed = 1.0f;

    [Header("Audio (optional)")]
    public AudioSource audioSource;
    public AudioClip[] windClips;
    [Range(0f, 2f)] public float windVolume = 1f;

    MaterialPropertyBlock mpb;

    public static WindController Instance { get; private set; }

    public float CurrentInfluence { get; private set; }
    public float CurrentSpeed { get; private set; }


    struct GustRequest
    {
        public float duration;
        public float influence;
        public float speed;
        public bool playSound;

        public GustRequest(float duration, float influence, float speed, bool playSound)
        {
            this.duration = duration;
            this.influence = influence;
            this.speed = speed;
            this.playSound = playSound;
        }
    }

    readonly Queue<GustRequest> gustQueue = new Queue<GustRequest>();
    Coroutine queueRoutine;

    void Awake()
    {
        Instance = this;
        mpb = new MaterialPropertyBlock();
    }


    void OnEnable()
    {
        CollectTargets();
        ApplyToAll(baseInfluence, baseSpeed);
        // Debug.Log($"Collected targets: {targets?.Length ?? 0}");  // only for Debug
    }

    void OnDisable()
    {
        if (Instance == this) Instance = null;
    }


    void CollectTargets()
    {
        var list = new List<SpriteRenderer>();

        if (windGrassObjects)
            list.AddRange(windGrassObjects.GetComponentsInChildren<SpriteRenderer>(true));

        if (windGrassObjectsSlowSway)
            list.AddRange(windGrassObjectsSlowSway.GetComponentsInChildren<SpriteRenderer>(true));

        targets = list.ToArray();
        // Debug.Log($"WindController: collected {targets.Length} sprites from the 2 roots.");
    }
   

    public void TriggerGust(float durationSeconds, float influence, float speed, bool playSound = true)
    {
        // Debug.Log($"TriggerGust called: duration={durationSeconds} influence={influence} speed={speed}");

        if (targets == null || targets.Length == 0)
            CollectTargets();

        gustQueue.Enqueue(new GustRequest(durationSeconds, influence, speed, playSound));

        if (queueRoutine == null)
            queueRoutine = StartCoroutine(ProcessQueue());
    }



    IEnumerator ProcessQueue()
    {
        while (gustQueue.Count > 0)
        {
            var gust = gustQueue.Dequeue();
            yield return StartCoroutine(GustRoutine(gust.duration, gust.influence, gust.speed, gust.playSound));
        }

        queueRoutine = null;
    }

    IEnumerator GustRoutine(float durationSeconds, float influence, float speed, bool playSound)
    {
        if (playSound) PlayRandomWind();

        float rampUp = 0.25f;
        float rampDown = 1.25f;

        rampUp = Mathf.Min(rampUp, durationSeconds * 0.3f);
        rampDown = Mathf.Min(rampDown, durationSeconds * 0.5f);

        // Ramp up
        yield return StartCoroutine(LerpWind(baseInfluence, baseSpeed, influence, speed, rampUp));

        // Hold
        float hold = Mathf.Max(0f, durationSeconds - rampUp - rampDown);
        if (hold > 0f)
        {
            ApplyToAll(influence, speed);
            yield return new WaitForSeconds(hold);
        }

        // Ramp down
        yield return StartCoroutine(LerpWind(influence, speed, baseInfluence, baseSpeed, rampDown));
    }

    IEnumerator LerpWind(float fromInfluence, float fromSpeed, float toInfluence, float toSpeed, float duration)
    {
        if (duration <= 0f)
        {
            ApplyToAll(toInfluence, toSpeed);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            k = Mathf.SmoothStep(0f, 1f, k);

            float inf = Mathf.Lerp(fromInfluence, toInfluence, k);
            float spd = Mathf.Lerp(fromSpeed, toSpeed, k);

            // I am trying to avoid too strong "jittering" close to the target value
            if (Mathf.Abs(spd - toSpeed) < 0.02f) spd = toSpeed;
            if (Mathf.Abs(inf - toInfluence) < 0.001f) inf = toInfluence;

            ApplyToAll(inf, spd);
            yield return null;
        }

        ApplyToAll(toInfluence, toSpeed);
    }

    void ApplyToAll(float influence, float speed)
    {
        CurrentInfluence = influence;
        CurrentSpeed = speed;

        if (targets == null || targets.Length == 0)
        {
            CollectTargets();
            if (targets == null || targets.Length == 0) return;
        }

        foreach (var r in targets)
        {
            if (!r) continue;

            r.GetPropertyBlock(mpb);
            mpb.SetFloat(influenceProperty, influence);
            mpb.SetFloat(speedProperty, speed);
            r.SetPropertyBlock(mpb);
        }
    }



    void PlayRandomWind()
    {
        if (!audioSource || windClips == null || windClips.Length == 0) return;

        var clip = windClips[Random.Range(0, windClips.Length)];
        if (clip) audioSource.PlayOneShot(clip, windVolume);
    }
}
