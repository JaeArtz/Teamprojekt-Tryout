using UnityEngine;

public class TriggerDispatcher : MonoBehaviour
{
    [Tooltip("Drag child object in here that has follow Target Simple and the animation")]
    public FollowTargetTrackPoint targetScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) targetScript.ActivateFollow(other.transform.root);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) targetScript.isFollowing = false;
    }
}