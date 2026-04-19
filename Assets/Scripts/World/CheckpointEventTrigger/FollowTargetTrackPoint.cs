using UnityEngine;

public class FollowTargetTrackPoint : MonoBehaviour
{
    [Header("Settings")]
    public Transform target; // Drag in Player here
    public Vector3 offset;
    public bool isFollowing = false;

    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;
        
    [SerializeField] float smoothSpeed = 1f;

    // Position for "Default Movement"
    private Vector3 homePosition2;
    private Vector3 velocity;

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
            transform.position = Vector3.SmoothDamp(transform.position, targetDestination, ref velocity, smoothSpeed);  
        }
        else
        {
            targetDestination = transform.position;
            if (followX) targetDestination.x = target.position.x + offset.x;
            if (followY) targetDestination.y = target.position.y + offset.y;


            Vector3 differenceBetweenTwoPoints = targetDestination - transform.position;
            transform.position += differenceBetweenTwoPoints * Time.deltaTime * smoothSpeed;
        }


        //  bei Lerp letzter Wert nur zwischen 0 und 1, und 1 = sei bei Target Destination, 0 =sei bei targetDestination
        // transform.position = Vector3.Lerp(transform.position, targetDestination, Time.deltaTime * smoothSpeed);

        // Kontroll-Log (Nur wenn isFollowing aktiv ist)
        if (isFollowing)
        {
            // Debug.Log("Ich gleite gemütlich zu: " + targetDestination);
        }
    }

    // Wird vom TriggerDispatcher aufgerufen
    public void ActivateFollow(Transform newTarget)
    {
        Debug.Log("ActivateFollow has been activated.");

        isFollowing = true;

    }

    [ContextMenu("Set Current Offset")]
    private void SetCurrentOffset()
    {
        if (target != null)
            offset = transform.position - target.position;
    }
}