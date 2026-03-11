using UnityEngine;

public class SinkingGround : MonoBehaviour
{

    [Header("Sink Settings")]
    [Tooltip("slows player movement")]
    [SerializeField] private float slowFactor = 0.3f; // 0.3f = 30% of normal velocity

    [Tooltip("restricts jumping height/force")]
    [SerializeField] private float jumpFactor = 0.2f; // 0.2f = almost no jumping possibly

    [Tooltip("slowly pulls player into SinkingGround")]
    [SerializeField] private float sinkForce = -0.5f; // slowly pulls palyer down, and causes damage, until avatar reaches checkpoint and teleports

    [Tooltip("time window for jumping while in SinkingGround (Coyote Time override)")]
    [SerializeField] private float sinkingGroundCoyouteTime = 0.2f;

    private float _originalMaxX;
    private float _originalMaxY;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            // gets original values
            _originalMaxX = player.MaxVelocityX;
            _originalMaxY = player.MaxVelocityY;

            // change values (while stuck in SinkingGround)
            player.MaxVelocityX = _originalMaxX * slowFactor;
            player.MaxVelocityY = _originalMaxY * jumpFactor;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            // if player doesn't actively jump and try to get out
            // he gets dragged down
            if (player.VerticalVelocity <= 0.1f)
            {
                player.VerticalVelocity = sinkForce;
            }

            player.remoteAccessToGroundCoyoteCounter = sinkingGroundCoyouteTime;

            player.MaxVelocityX = _originalMaxX * slowFactor;
            player.MaxVelocityY = _originalMaxY * jumpFactor;

            player.VerticalVelocity = sinkForce;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponentInParent<PlayerMovement>();
        if (player != null)
        {
            // sets values back to normal
            player.MaxVelocityX = _originalMaxX;
            player.MaxVelocityY = _originalMaxY;
        }
    }
}

