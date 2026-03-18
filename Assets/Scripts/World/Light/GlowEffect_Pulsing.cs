using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowEffect_Pulsing : MonoBehaviour
{
    [Header("GlowPulse Settings")]
    [Tooltip("Minimum Transparency of ColorOverlay")]
    public float minAlpha = 0.3f;
    [Tooltip("Maximum Transparency of ColorOberlay")]
    public float maxAlpha = 0.8f;
    [Tooltip("Influence value for sinus calculation of PulseFrequency")]
    public float PulseFrequency = 2f;

    private SpriteRenderer sr;
    private Color baseColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * PulseFrequency) + 1f) / 2f;
        float a = Mathf.Lerp(minAlpha, maxAlpha, t);
        var c = baseColor;
        c.a = a;
        sr.color = c;
    }
}


