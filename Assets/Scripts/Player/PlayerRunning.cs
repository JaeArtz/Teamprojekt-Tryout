using UnityEngine;

public class PlayerRunning : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum horizontal velocity of player")] private float playerMaxVelocityX = 10f;
    [SerializeField, Tooltip("Accelerates player to max movement speed. Acceleration speed is applied when player movement direction equals keyboard input direction")]
    private float playerAccelerationX = 5f;
    public float MaxVelocityX { get => playerMaxVelocityX; set => playerMaxVelocityX = value; }

    private float walljumpVelocityX;

    [SerializeField, Tooltip("Decelerates player to movement speed 0. Deceleration speed is applied when player movement direction is different to keyboard input direction")]
    private float playerDecelerationX = 15f;

    private Rigidbody2D body;
    public Rigidbody2D Body { set { body = value; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        PlayerWallActions wallActions = GetComponent<PlayerWallActions>();
        if (!wallActions)
        {
            Debug.LogError("PlayerWallActions Script not found!");
            return;
        }

        walljumpVelocityX = wallActions.PlayerMaxWallJumpVelocityX;
    }

    public void ApplyNormalMovement(float horizontalInput)
    {
        Move(horizontalInput);
    }

    public void ApplyWalljumpMovement(float horizontalInput, float t)
    {
        float manipulator = Mathf.Lerp(1f, 0f, t);
        float wallJumpVelocity = Mathf.Lerp(playerMaxVelocityX, walljumpVelocityX, t);
        Move(horizontalInput, maxVelocityX: wallJumpVelocity, accelerationRateManipulator: manipulator, decelerationRateManipulator: manipulator);
    }

    public void ApplyRollMovement(float horizontalInput, float targetSpeedManipulator, float decelerationRateManipulator)
    {
        Move(horizontalInput, targetSpeedManipulator: targetSpeedManipulator, accelerationRateManipulator: 10f, decelerationRateManipulator: decelerationRateManipulator);
    }

    private void Move(float horizontalInput, float? maxVelocityX = null, float targetSpeedManipulator = 1, float accelerationRateManipulator = 1, float decelerationRateManipulator = 1)
    {
        if (!maxVelocityX.HasValue)
            maxVelocityX = playerMaxVelocityX;
        float targetSpeed = horizontalInput * (float)maxVelocityX * targetSpeedManipulator;
        float accelRate = Mathf.Sign(body.linearVelocityX) == Mathf.Sign(targetSpeed) && horizontalInput != 0 ? playerAccelerationX * accelerationRateManipulator : playerDecelerationX * decelerationRateManipulator;
        float speedDiff = targetSpeed - body.linearVelocity.x;
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;

        body.linearVelocity = new Vector2(
            Mathf.Clamp(body.linearVelocity.x + movement, -(float)maxVelocityX * targetSpeedManipulator, (float)maxVelocityX * targetSpeedManipulator),
            body.linearVelocity.y
        );
    }
}
