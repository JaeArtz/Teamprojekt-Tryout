using UnityEngine;
using System.Collections;

public class SoulPickupPhoenix : MonoBehaviour
{
    [Header("Objects")]
    public GameObject phoenixObject;
    public Transform phoenixTargetPoint;
    public GameObject camTrackerPoint;
    public LevelLoaderScript levelLoader;

    [Header("Teleportation")]
    public Transform playerTeleportPoint;      // Punkt 1 (Sand)
    public Transform playerTeleportPointGate; // Punkt 2 (Gate)

    [Header("Phoenix Settings")]
    public float phoenixSize = 3.0f;
    public float spawnHeightOffset = 1.5f;

    [Header("Timing")]
    public float fadeDurationPhoenix = 1.5f;
    public float cameraFollowDuration = 1.5f;
    public float hoverTime = 0.5f;
    public float flightDuration = 3.0f;

    private bool collectedPhoenix = false;

    private void Start()
    {
        if (phoenixObject != null) phoenixObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collectedPhoenix || !other.CompareTag("Player")) return;
        collectedPhoenix = true;

        // --- DER ABSOLUTE FIX ---
        // Wir suchen das Objekt mit dem Namen "Player". 
        // Falls der Trigger am 'Scaler' sitzt, wandern wir zum Parent.
        GameObject thePlayer = null;
        if (other.gameObject.name == "Player")
        {
            thePlayer = other.gameObject;
        }
        else
        {
            thePlayer = other.transform.parent.gameObject;
        }

        // Falls er immer noch den Scaler hat (Sicherheitscheck)
        if (thePlayer.name == "Scaler") thePlayer = thePlayer.transform.parent.gameObject;

        // Physik stoppen
        var rb = thePlayer.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 1. TELEPORT: Sofort zu Punkt 1 (Sand)
        if (playerTeleportPoint != null)
        {
            thePlayer.transform.position = playerTeleportPoint.position;
            Physics2D.SyncTransforms();
        }

        StartCoroutine(PhoenixFinalSequence(thePlayer));
    }

    private IEnumerator PhoenixFinalSequence(GameObject player)
    {
        // Phönix Spawn Logik
        if (phoenixObject != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y += spawnHeightOffset;
            phoenixObject.transform.position = spawnPos;
            phoenixObject.transform.localScale = new Vector3(-phoenixSize, phoenixSize, 1f);
            phoenixObject.SetActive(true);

            if (camTrackerPoint != null)
            {
                camTrackerPoint.transform.parent = phoenixObject.transform;
                camTrackerPoint.transform.localPosition = Vector3.zero;
            }
        }

        // Seele am Boden ausmachen
        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(fadeDurationPhoenix);

        // --- JETZT: DEN PLAYER DEAKTIVIEREN ---
        // Wir geben hier explizit das 'player' Objekt aus der OnTriggerEnter mit.
        Debug.Log("Deaktiviere Objekt: " + player.name);
        player.SetActive(false);

        yield return new WaitForSeconds(hoverTime);
        StartCoroutine(DetachCameraAfterDelay());

        // Flug des Phönix
        if (phoenixObject != null && phoenixTargetPoint != null)
        {
            Vector3 startPos = phoenixObject.transform.position;
            Vector3 endPos = phoenixTargetPoint.position;
            float elapsedTime = 0;
            while (elapsedTime < flightDuration)
            {
                phoenixObject.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / flightDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            phoenixObject.transform.position = endPos;

            // --- 2. TELEPORT ZUM GATE ---
            if (playerTeleportPointGate != null)
            {
                player.transform.position = playerTeleportPointGate.position;
                Physics2D.SyncTransforms();
            }
        }

        // --- PLAYER WIEDER AKTIVIEREN ---
        player.SetActive(true);

        // Level Loader triggern
        if (levelLoader != null)
        {
            levelLoader.LoadNextLevel();
        }

        gameObject.SetActive(false);
    }

    private IEnumerator DetachCameraAfterDelay()
    {
        yield return new WaitForSeconds(cameraFollowDuration);
        if (camTrackerPoint != null) camTrackerPoint.transform.parent = null;
    }
}