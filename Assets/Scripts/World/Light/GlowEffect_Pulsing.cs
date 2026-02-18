using UnityEngine;

public class GlowEffect_Pulsing : MonoBehaviour
{
   
    public float minAlpha = 0.3f;
    public float maxAlpha = 0.8f;
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


