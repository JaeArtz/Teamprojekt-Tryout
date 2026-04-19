using UnityEngine;

public class FollowTargetExperimental : MonoBehaviour
{
    [Header("Settings")]
    public Transform target; // Drag in Player here
    public Vector3 offset;
    public bool isFollowing = false;

    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;

    // Position for "Default Movement"
    private Vector3 homePosition2;

    private void Start()
    {
        homePosition2 = transform.position;
        // GEMINI: Test-Log
        Debug.Log("GEMINI: Start auf " + gameObject.name + ". Home Position ist: " + homePosition2);
    }

    private void LateUpdate()
    {
        
        if (target == null) return;

        Vector3 targetDestination;

        if (!isFollowing)
        {
            targetDestination = homePosition2;
        }
        else
        {
            
            targetDestination = transform.position;
            if (followX) targetDestination.x = target.position.x + offset.x;
            if (followY) targetDestination.y = target.position.y + offset.y;
        }

        
        transform.position = targetDestination;

        // GEMINI: Kontroll-Log (Nur wenn isFollowing aktiv ist)
        if (isFollowing)
        {
            Debug.Log("Ich sollte mich bewegen zu: " + targetDestination);
        }
    }

    // Wird vom TriggerDispatcher aufgerufen
    public void ActivateFollow(Transform newTarget)
    {
        isFollowing = true;
        target = newTarget;
        Debug.Log("GEMINI: ActivateFollow wurde aufgerufen!");
    }

    [ContextMenu("Set Current Offset")]
    private void SetCurrentOffset()
    {
        if (target != null)
            offset = transform.position - target.position;
    }
}