using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AutoPilot : MonoBehaviour
{
    [Header("Ziele")]
    [Tooltip("EndPoint at left side")]
    public Transform flightTargetPoint;
    [Tooltip("Point in Scene for Camera, will be picked up along the way")]
    public Transform camTrackerPoint;
    [Tooltip("Drag PlayerObject here that needs to be deactivated")]
    public GameObject playerObject;
    [Header("Timing")]
    [Tooltip("Total of Flight Duration of the Phoenix in seconds")]
    public float flightDuration = 5.0f;
    [Tooltip("After which percentage of total Travel will Cam be picked up? (0.3 = 30%)")]
    public float pickupTimePercent = 0.3f;
    [Tooltip("How many seconds will Cam be carried by Phnoenix?")]
    public float holdDurationSeconds = 2.0f;

    private GameObject UI;
    private GameObject myLevelLoader;
    private bool cameraAttached = false;

    private void Awake()
    {
        UI = GameObject.Find("HUDCanvas");
        myLevelLoader = GameObject.Find("LevelLoader");
    }

    private void Start()
    {
        StartCoroutine(CompleteFlightSequence());
        playerObject.SetActive(false);
        UI.SetActive(false);
    }

    IEnumerator CompleteFlightSequence()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = flightTargetPoint.position;
        float elapsedTime = 0;

        // calculation of PickupTime for Cam
        float pickupTime = flightDuration * pickupTimePercent;

        while (elapsedTime < flightDuration)
        {
            // 1. continuous movement towards EndPoint, by Phoenix
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / flightDuration);

            // 2. Checks if it is already time for Pickup
            if (!cameraAttached && elapsedTime >= pickupTime)
            {
                AttachCamera();
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Endpoint reached
        transform.position = endPos;
        LoadNextLevel();
    }

    void AttachCamera()
    {
        if (camTrackerPoint != null)
        {
            cameraAttached = true;
            camTrackerPoint.SetParent(this.transform, true);
            

            // Timer to let go of Cam
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