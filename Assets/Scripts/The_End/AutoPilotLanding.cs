using UnityEngine;
using System.Collections;

public class AutoPilotLanding : MonoBehaviour
{
    [Header("Referenzen")]
    public Transform flightTargetPoint; // Hier ziehst du das (deaktivierte) Player-Objekt rein
    public GameObject playerObject;     // Das Player-Objekt, das aktiviert werden soll

    [Header("Einstellungen")]
    public float flightDuration = 3.0f; // Wie lange braucht der Phönix zum Spieler?

    private void Start()
    {
        // Der Landeanflug startet sofort
        StartCoroutine(StartLandingSequence());
    }

    IEnumerator StartLandingSequence()
    {
        Vector3 startPos = transform.position;

        // Wir nehmen die Position des Players/Targets als Ziel
        Vector3 endPos = flightTargetPoint.position;
        float elapsedTime = 0;

        // 1. Der Flug zum Spieler
        while (elapsedTime < flightDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / flightDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Exakt auf den Zielpunkt setzen
        transform.position = endPos;

        // 2. Der "Kniff": Phönix weg, Player da
        if (playerObject != null)
        {
            playerObject.SetActive(true);

            // Falls der Player ein Rigidbody hat, kurz sicherstellen, 
            // dass er nicht durch den Boden fällt oder komisch wegspringt
            var rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null) rb.WakeUp();
        }

        // Phönix (dieses Objekt) deaktivieren
        this.gameObject.SetActive(false);
    }
}