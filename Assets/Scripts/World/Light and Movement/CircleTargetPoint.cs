using UnityEngine;

public class CircleTargetPoint : MonoBehaviour
{
    [Header("Settings")]
    // GEMINI: Hier ziehst du im Inspektor den "PlayerTracker" rein
    public Transform targetPoint;

    [Header("Circle Movement")]
    public float radiusWidth = 1f;
    public float radiusHeight = 0.5f;
    public float movingSpeed = 1f;
    public float offset = 0f;

    void Update()
    {
        // GEMINI: Falls kein TargetPoint zugewiesen wurde, brechen wir ab
        if (targetPoint == null) return;

        // Die mathematische Kreisberechnung
        float t = Time.time * movingSpeed + offset;
        float x = Mathf.Cos(t) * radiusWidth;
        float y = Mathf.Sin(t) * radiusHeight;

        // GEMINI: Hier kombinieren wir die Position des Trackers mit der Kreisbahn
        // Da wir das jetzt im Weltraum (transform.position) machen, 
        // folgen die Fische dem TrackerPoint sanft, während sie kreisen.
        transform.position = targetPoint.position + new Vector3(x, y, 0f);
    }
}