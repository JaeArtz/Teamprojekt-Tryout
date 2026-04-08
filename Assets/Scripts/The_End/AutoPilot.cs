using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AutoPilot : MonoBehaviour
{
    [Header("Ziele")]
    public Transform flightTargetPoint; // Endstation ganz links
    public Transform camTrackerPoint;   // Der Punkt, der aufgepickt wird

    [Header("Timing")]
    public float flightDuration = 5.0f;     // Gesamtzeit des Flugs (Start bis Ziel)
    public float pickupTimePercent = 0.3f;  // Wann (0.0 - 1.0) wird die Kamera gegriffen? (0.3 = nach 30% der Strecke)
    public float holdDurationSeconds = 2.0f; // Wie viele Sekunden wird sie festgehalten?

    private GameObject myLevelLoader;
    private bool cameraAttached = false;

    private void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");
    }

    private void Start()
    {
        StartCoroutine(CompleteFlightSequence());
    }

    IEnumerator CompleteFlightSequence()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = flightTargetPoint.position;
        float elapsedTime = 0;

        // Zeitpunkt berechnen, an dem die Kamera gegriffen wird
        float pickupTime = flightDuration * pickupTimePercent;

        while (elapsedTime < flightDuration)
        {
            // 1. Bewege den Vogel kontinuierlich Richtung Ziel
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / flightDuration);

            // 2. Prüfen: Ist es Zeit, die Kamera aufzugreifen?
            if (!cameraAttached && elapsedTime >= pickupTime)
            {
                AttachCamera();
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Am Ziel angekommen
        transform.position = endPos;
        LoadNextLevel();
    }

    void AttachCamera()
    {
        if (camTrackerPoint != null)
        {
            cameraAttached = true;
            camTrackerPoint.SetParent(this.transform, true);
            // Optional: camTrackerPoint.localPosition = Vector3.zero; // Falls er exakt auf den Vogel schnappen soll

            // Timer zum Loslassen starten
            Invoke(nameof(DetachCamera), holdDurationSeconds);
        }
    }

    void DetachCamera()
    {
        if (camTrackerPoint != null)
        {
            camTrackerPoint.SetParent(null);
        }
    }

    void LoadNextLevel()
    {
        if (myLevelLoader != null)
        {
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}