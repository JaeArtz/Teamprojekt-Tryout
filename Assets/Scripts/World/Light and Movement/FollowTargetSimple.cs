using UnityEngine;

public class FollowTargetExperimental2 : MonoBehaviour
{
    [Header("Settings")]
    public Transform target; // Drag in Player here
    public Vector3 offset;
    public bool isFollowing = false; // difference to first FollowTarget-Script

    [SerializeField] bool followX = true;
    [SerializeField] bool followY = true;

    // Position for "Default Movement", before and after PlayerFollow
    private Vector3 homePosition2;

    private void Start()
    {
        // homePosition2 = the default point
        homePosition2 = transform.position;
        Debug.Log("Start auf " + gameObject.name + ". Home Position ist: " + homePosition2);
    }

    // WHATEVER collides, will make sure the Player ist the Target
    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (target != null)
        {
            isFollowing = true;
           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isFollowing = false;        
    }

    private void LateUpdate()
    {
        // if no target in sight or following is disabled, return to home position
        if (target == null || !isFollowing)
        {
            // smoothly moves back to home position for default MOvement
            transform.position = Vector3.Lerp(transform.position, homePosition2, Time.deltaTime * 2f);
            return;
        }

        Vector3 newPosition = transform.position;

        // Target Position Cordinates are taken here to follow
        if (followX) newPosition.x = target.position.x + offset.x;
        if (followY) newPosition.y = target.position.y + offset.y;

        transform.position = newPosition;
        // transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 2f); ;
    }

    // Setting the Target:
    public void ActivateFollow(Transform newTarget)
    {        
        isFollowing = true;
    }

    [ContextMenu("Set Current Offset")]
    private void SetCurrentOffset()
    {
        if (target != null)
            offset = transform.position - target.position;
    }
}