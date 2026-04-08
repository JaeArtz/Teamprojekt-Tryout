using UnityEngine;

public class LightAttached : MonoBehaviour
{
    [Header("Kreisbewegung (Relativ zum Parent)")]
    public float radiusWidth = 1f;
    public float radiusHeight = 0.5f;
    public float movingSpeed = 2f;
    public float offset = 0f;

    [Header("Pulsieren")]
    public bool usePulse = true;
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.2f; // Wie stark soll die Größe schwanken?
    private Vector3 originalScale;

    void Start()
    {
        // Wir merken uns die Größe, die du im Inspector eingestellt hast
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 1. KREISBEWEGUNG
        // Wir nutzen localPosition, damit die Lichter IMMER am Player kleben,
        // egal wohin er läuft oder fliegt.
        float t = (Time.time * movingSpeed) + offset;
        float x = Mathf.Cos(t) * radiusWidth;
        float y = Mathf.Sin(t) * radiusHeight;

        transform.localPosition = new Vector3(x, y, 0f);

        // 2. PULSIEREN (Größe)
        if (usePulse)
        {
            // Erzeugt einen Wert, der sanft zwischen (1 - pulseAmount) und (1 + pulseAmount) schwankt
            float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed + offset) * pulseAmount;
            transform.localScale = originalScale * scaleFactor;
        }
    }
}