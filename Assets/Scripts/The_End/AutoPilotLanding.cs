using UnityEngine;
using System.Collections;

public class AutoPilotLanding : MonoBehaviour
{
    [Header("Referenzen")]
    [Tooltip("Drag (deactivated) PlayerObject in here")]
    public Transform flightTargetPoint;
    [Tooltip("Drag PlayerObject here that needs to be deactivated")]
    public GameObject playerObject;

    [Header("Settings")]
    [Tooltip("TravelTime in Seconds of Phoenix (SpawnPoint) towards Player (deactivated).")]
    public float flightDuration = 3.0f;

    public void Awake()
    {
        if(SaveSystem.LoadData().wasLoaded == true)
        {
            this.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        playerObject.SetActive(false);
        // Scene starts with LandingSequence
        // "Phoenix places Player on Ground"
        StartCoroutine(StartLandingSequence());
    }

    IEnumerator StartLandingSequence()
    {
        Vector3 startPos = transform.position;

        // position of Player is GoalPoint
        Vector3 endPos = flightTargetPoint.position;
        float elapsedTime = 0;

        // 1. Flight towards Player
        while (elapsedTime < flightDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / flightDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Lands exactly on Point
        transform.position = endPos;

        // 2. Switch: Phoenix disappears, Player re-activates
        if (playerObject != null)
        {
            playerObject.SetActive(true);
             
            // checks for Player-RigidBody, security measure
            var rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null) rb.WakeUp();
        }

        // deactivates Phoenix
        this.gameObject.SetActive(false);
    }
}