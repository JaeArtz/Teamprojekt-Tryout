using UnityEngine;

public class SinkingGround : MonoBehaviour
{

    [Header("Sink Settings")]
    [Tooltip("slows player movement")]
    [SerializeField] private float slowFactor = 0.7f; // 0.3f = 30% of normal velocity

    [Tooltip("restricts jumping height/force")]
    [SerializeField] private float jumpFactor = 0.9f; // 0.2f = almost no jumping possibly

    [Tooltip("slowly pulls player into SinkingGround")]
    [SerializeField] private float sinkForce = -0.2f; // slowly pulls palyer down, and causes damage, until avatar reaches checkpoint and teleports

    [Tooltip("time window for jumping while in SinkingGround (Coyote Time override)")]
    [SerializeField] private float sinkingGroundCoyouteTime = 0.7f;

    // value can be null if not set yet, so: make it nullable
    // thats why readonly doesn't work here
    private float? _originalMaxX;
    private float? _originalMaxY;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var jump = other.GetComponentInParent<PlayerJump>();
        var run = other.GetComponentInParent<PlayerRunning>();
        if (jump && run)
        {
            // gets original values
            if(!_originalMaxX.HasValue)
                _originalMaxX = run.MaxVelocityX;
            if(!_originalMaxY.HasValue)
                _originalMaxY = jump.MaxVelocityY;

            // change temporary values (while stuck in SinkingGround)
            // NO changing of original values
            run.MaxVelocityX = (float)_originalMaxX * slowFactor;
            jump.MaxVelocityY = (float)_originalMaxY * jumpFactor;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var jump = other.GetComponentInParent<PlayerJump>();
        var run = other.GetComponentInParent<PlayerRunning>();
        var body = other.GetComponentInParent<Rigidbody2D>();
        if (jump && run && body)
        {
            // if player doesn't actively jump and try to get out
            // he gets dragged down
            if (body.linearVelocityY <= sinkForce)
            {
                body.linearVelocityY = sinkForce;
            }

            jump.remoteAccessToGroundCoyoteCounter = sinkingGroundCoyouteTime;

            run.MaxVelocityX = (float)_originalMaxX * slowFactor;
            jump.MaxVelocityY = (float)_originalMaxY * jumpFactor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var jump = other.GetComponentInParent<PlayerJump>();
        var run = other.GetComponentInParent<PlayerRunning>();
        if (jump && run)
        {
            // change values (while stuck in SinkingGround)
            run.MaxVelocityX = (float)_originalMaxX;
            jump.MaxVelocityY = (float)_originalMaxY;
        }
    }
}

