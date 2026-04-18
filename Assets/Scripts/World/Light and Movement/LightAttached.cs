using UnityEngine;

public class LightAttached : MonoBehaviour
{
    [Header("Circular Movement (in relation to Parent)")]
    public float radiusWidth = 1f;
    public float radiusHeight = 0.5f;
    public float movingSpeed = 2f;
    public float offset = 0f;

    [Header("Pulsating")]
    public bool usePulse = true;
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.2f;
    private Vector3 originalScale;

    void Start()
    {
        // Value set in Inspector for size of Light
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 1. Circle Movement
        // localPosition, so Light stay on Player (/Object)
        // wherever he goes
        float t = (Time.time * movingSpeed) + offset;
        float x = Mathf.Cos(t) * radiusWidth;
        float y = Mathf.Sin(t) * radiusHeight;

        transform.localPosition = new Vector3(x, y, 0f);

        // 2. Pulsating of Light (with given size, originalScale, from Inspector)
        if (usePulse)
        {
            // Pulsating bewteen (1 - pulseAmount) and (1 + pulseAmount)
            float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed + offset) * pulseAmount;
            transform.localScale = originalScale * scaleFactor;
        }
    }
}