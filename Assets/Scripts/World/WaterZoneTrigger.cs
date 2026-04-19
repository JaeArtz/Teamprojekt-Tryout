using UnityEngine;

public class WaterZoneTrigger : MonoBehaviour
{
    [Header("Assignment")]
    [Tooltip("Drag Parentcontainer with fish (or other) animation- childObject in here.")]
    public FollowTargetExperimental2 fishSwarm;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // We look for the "Player" tag on the object itself or anywhere in its parents
        Transform rootPlayer = FindPlayerRoot(other.transform);

        if (rootPlayer != null)
        {
            if (fishSwarm != null)
            {
                // assign followTarget to Swarm (using the found root player)
                fishSwarm.ActivateFollow(rootPlayer);
                Debug.Log("Fische: Target gesetzt auf " + rootPlayer.name);
            }
        }
    }

    // Fish stay back when player leaves
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the exiting object belongs to the Player
        if (FindPlayerRoot(other.transform) != null)
        {
            if (fishSwarm != null)
            {
                fishSwarm.isFollowing = false;
                Debug.Log("Fische: Player hat die Zone verlassen.");
            }
        }
    }

    // Helper: Searches up the hierarchy for the "Player" tag
    private Transform FindPlayerRoot(Transform current)
    {
        if (current.CompareTag("Player")) return current;
        if (current.parent != null) return FindPlayerRoot(current.parent);
        return null;
    }
}