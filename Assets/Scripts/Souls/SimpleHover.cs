using UnityEngine;

public class SimpleHover : MonoBehaviour
{
    [Header("Hover Settings")]
    public float amplitude = 0.5f; // Wie weit geht es hoch/runter?
    public float speed = 2f;      // Wie schnell ist die Bewegung?
    public float timeOffset;      // Damit nicht alle Geister synchron schweben

    private Vector3 startPos;

    void Start()
    {
        // Wir speichern die Startposition relativ zum Parent
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Berechnung der neuen Y-Position mit Sinus
        float newY = startPos.y + Mathf.Sin((Time.time + timeOffset) * speed) * amplitude;

        // Anwendung auf die lokale Position
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}