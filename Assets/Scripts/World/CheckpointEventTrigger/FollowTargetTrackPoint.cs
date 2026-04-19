using UnityEngine;

public class FollowTargetTrackPoint : MonoBehaviour
{
    [Header("Settings")]
    public Transform target; // Drag in Player here
    public Vector3 offset;
    public bool isFollowing = false;

    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;
        
    [SerializeField] float smoothSpeed = 2f;

    // Position for "Default Movement"
    private Vector3 homePosition2;

    private void Start()
    {
        homePosition2 = transform.position;       
        
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
        
        transform.position = Vector3.Lerp(transform.position, targetDestination, Time.deltaTime * smoothSpeed);

        // Kontroll-Log (Nur wenn isFollowing aktiv ist)
        if (isFollowing)
        {
            // Debug.Log("Ich gleite gemütlich zu: " + targetDestination);
        }
    }

    // Wird vom TriggerDispatcher aufgerufen
    public void ActivateFollow(Transform newTarget)
    {
        isFollowing = true;
        target = newTarget;        
    }

    [ContextMenu("Set Current Offset")]
    private void SetCurrentOffset()
    {
        if (target != null)
            offset = transform.position - target.position;
    }
}