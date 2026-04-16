using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class SoulPickupPhoenix : MonoBehaviour
{
    [Header("Objects")]
    public GameObject phoenixObject;
    public Transform phoenixTargetPoint;
    public GameObject camTrackerPoint;
    private GameObject myLevelLoader;
    public PlayerController playerController;
    

    [Header("Teleportation")]
    public Transform playerTeleportPoint;      // Point 1 (Sand)
    public Transform playerTeleportPointGate; // Point 2 (Gate)

    [Header("Phoenix Settings")]
    public float phoenixSize = 3.0f;
    public float spawnHeightOffset = 1.5f;

    [Header("Timing")]
    public float fadeDurationPhoenix = 1.5f;
    public float cameraFollowDuration = 1.5f;
    public float hoverTime = 0.5f;
    public float flightDuration = 3.0f;

    private bool collectedPhoenix = false;
    private GameObject HUD;
    private GameObject pauseCanvas;
    private void Awake()
    {
        HUD = GameObject.Find("HUDCanvas");
        pauseCanvas = GameObject.Find("PauseCanvas");
        myLevelLoader = GameObject.Find("LevelLoader");
    }
    private void Start()
    {
        if (phoenixObject != null) phoenixObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collectedPhoenix || !other.CompareTag("Player")) return;
        collectedPhoenix = true;

        // playerController.SetInputLocked(true);

        // Looks for Player, if necessary at Parent        
        GameObject thePlayer = null;
        if (other.gameObject.name == "Player")
        {
            thePlayer = other.gameObject;
        }
        else
        {
            thePlayer = other.transform.parent.gameObject;
        }

        if (thePlayer.name == "Scaler") thePlayer = thePlayer.transform.parent.gameObject;

        // stop physics at Player
        var rb = thePlayer.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 1. TELEPORT: to Point 1 (Sand)
        if (playerTeleportPoint != null)
        {
            thePlayer.transform.position = playerTeleportPoint.position;
            Physics2D.SyncTransforms();
        }
        HUD.SetActive(false);
        pauseCanvas.SetActive(false);
        StartCoroutine(PhoenixFinalSequence(thePlayer));
    }

    private IEnumerator PhoenixFinalSequence(GameObject player)
    {
        // Phoenix Spawn Logic
        
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

        // Turns Off Souls
        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(fadeDurationPhoenix);

        //Deactivates Player
        Debug.Log("Deaktiviere Objekt: " + player.name);
        player.SetActive(false);
        yield return new WaitForSeconds(hoverTime);
        StartCoroutine(DetachCameraAfterDelay());

        // Flight Of The Phoenix
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

            // --- 2. TELEPORT to GATE ---
            if (playerTeleportPointGate != null)
            {
                player.transform.position = playerTeleportPointGate.position;
                Physics2D.SyncTransforms();
            }
        }

        // --- PLAYER REactivation ---
        player.SetActive(true);

        // triggers LevelLoader
        
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        

        gameObject.SetActive(false);
    }

    private IEnumerator DetachCameraAfterDelay()
    {
        yield return new WaitForSeconds(cameraFollowDuration);
        if (camTrackerPoint != null) camTrackerPoint.transform.parent = null;
    }
}